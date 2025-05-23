using System.Text.Json;
using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using PromptArchive.Configuration;
using PromptArchive.Database;
using PromptArchive.Services;
using Serilog;

var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
var loggerConfig = new LoggerConfiguration().ReadFrom.Configuration(config);

Log.Logger = loggerConfig.CreateLogger();

try
{
    Log.Information("Starting up...");
    var builder = WebApplication.CreateBuilder(args);

    builder.Logging.ClearProviders();
    builder.Logging.AddSerilog();

    var localUploadsDirectory = Path.Combine(builder.Environment.WebRootPath, "uploads/images");
    if (!Directory.Exists(localUploadsDirectory))
        Directory.CreateDirectory(localUploadsDirectory);

    builder.Services.Configure<LocalStorageSettings>(builder.Configuration.GetSection("Storage:LocalStorage"));
    builder.Services.Configure<S3StorageSettings>(builder.Configuration.GetSection("Storage:S3"));
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddSingleton<S3StorageService>();
    builder.Services.AddSingleton<LocalStorageService>();
    builder.Services.AddSingleton(StorageServiceFactory.CreateStorageService);

    builder.Services.AddDbContext<ApplicationDbContext>(c =>
        c.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
            b => b.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery)));

    builder.Services.AddIdentity<ApplicationUser, IdentityRole>(o =>
        {
            o.Password.RequireDigit = true;
            o.Password.RequireLowercase = true;
            o.Password.RequireUppercase = true;
            o.Password.RequireNonAlphanumeric = true;
            o.Password.RequiredLength = 8;

            o.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

    var authBuilder = builder.Services.AddAuthentication(o =>
    {
        o.DefaultScheme = IdentityConstants.ApplicationScheme;
        o.DefaultSignInScheme = IdentityConstants.ApplicationScheme;
    });

    var oauthSettings = builder.Configuration.GetSection("Authentication:OAuth").Get<OAuthSettings>();
    if (oauthSettings is not null)
    {
        authBuilder.AddOAuth("Generic", options =>
        {
            options.ClientId = oauthSettings.ClientId;
            options.ClientSecret = oauthSettings.ClientSecret;
            options.CallbackPath = "/";

            options.AuthorizationEndpoint = oauthSettings.AuthorizationEndpoint;
            options.TokenEndpoint = oauthSettings.TokenEndpoint;
            options.UserInformationEndpoint = oauthSettings.UserInfoEndpoint;
            options.SignInScheme = IdentityConstants.ExternalScheme;

            options.SaveTokens = true;

            options.Events = new OAuthEvents
            {
                OnCreatingTicket = async context =>
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                    request.Headers.Accept.Add(
                        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    request.Headers.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", context.AccessToken);

                    var response = await context.Backchannel.SendAsync(request,
                        HttpCompletionOption.ResponseHeadersRead, context.HttpContext.RequestAborted);
                    response.EnsureSuccessStatusCode();

                    var user = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
                    context.RunClaimActions(user.RootElement);
                },
                OnRemoteFailure = context =>
                {
                    context.Response.Redirect("/auth-failed");
                    context.HandleResponse();
                    return Task.CompletedTask;
                }
            };
        });
    }

    builder.Services.ConfigureApplicationCookie(o =>
    {
        o.Cookie.HttpOnly = true;
        o.Cookie.Name = "PromptArchive.Auth";
        o.Cookie.SameSite = SameSiteMode.Strict;
        o.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        o.ExpireTimeSpan = TimeSpan.FromDays(14);
        o.SlidingExpiration = true;

        o.LoginPath = "/login";
        o.LogoutPath = "/api/v1/auth/logout";

        o.Events.OnRedirectToLogin = context =>
        {
            if (context.Request.Path.StartsWithSegments("/api"))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            }

            context.Response.Redirect(context.RedirectUri);
            return Task.CompletedTask;
        };

        o.Events.OnRedirectToAccessDenied = context =>
        {
            if (context.Request.Path.StartsWithSegments("/api"))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return Task.CompletedTask;
            }

            context.Response.Redirect(context.RedirectUri);
            return Task.CompletedTask;
        };
    });

    builder.Services.AddAuthorization(o => { o.AddPolicy("Admin", p => { p.RequireRole("Admin"); }); });

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("VueFrontend", p =>
        {
            p.WithOrigins("http://localhost:5173", "http://localhost:5174")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
    });

    builder.Services
        .AddFastEndpoints()
        .SwaggerDocument(o =>
        {
            o.MaxEndpointVersion = 1;
            o.DocumentSettings = s =>
            {
                s.DocumentName = "Initial Release";
                s.Title = "PromptArchive API";
                s.Version = "v1";
            };
        });

    builder.Services.Configure<KestrelServerOptions>(o =>
    {
        o.Limits.MaxRequestBodySize = builder.Configuration.GetValue<int>("MaxUploadSize");
    });

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }
    else
    {
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseRouting();
    app.UseCors("VueFrontend");
    app.UseAuthentication();
    app.UseAuthorization();

    app.UseStaticFiles();

    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(localUploadsDirectory),
        RequestPath = "/images"
    });

    app.UseFastEndpoints(c =>
        {
            c.Versioning.Prefix = "v";
            c.Versioning.DefaultVersion = 1;
            c.Versioning.PrependToRoute = true;
            c.Endpoints.RoutePrefix = "api";

            c.Errors.ResponseBuilder = (failures, ctx, statusCode) =>
            {
                return new
                {
                    status = statusCode,
                    errors = failures.Select(f => f.ErrorMessage).ToList()
                };
            };
        })
        .UseSwaggerGen();

    app.MapFallbackToFile("index.html");

    await using (var scope = app.Services.CreateAsyncScope())
    {
        var services = scope.ServiceProvider;
        var environment = services.GetRequiredService<IHostEnvironment>();
        var dbContext = services.GetRequiredService<ApplicationDbContext>();

        await dbContext.Database.MigrateAsync();

        await IdentitySeeder.SeedBaseRoles(services);
        await IdentitySeeder.SeedAdminUserAsync(services);
    }

    await app.RunAsync();
}
catch (HostAbortedException)
{
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application startup failed");
}
finally
{
    await Log.CloseAndFlushAsync();
}