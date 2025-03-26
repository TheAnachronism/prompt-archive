using FastEndpoints;
using FastEndpoints.Security;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PromptArchive.Database;
using Serilog;
using YamlDotNet.Serialization;

var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
var loggerConfig = new LoggerConfiguration().ReadFrom.Configuration(config);

Log.Logger = loggerConfig.CreateLogger();

try
{
    Log.Information("Starting up...");
    var builder = WebApplication.CreateBuilder(args);

    builder.Logging.ClearProviders();
    builder.Logging.AddSerilog();
    
    builder.Services.AddSwaggerDocument()
        .AddEndpointsApiExplorer();

    builder.Services.AddDbContext<ApplicationDbContext>(c =>
        c.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

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

    builder.Services.AddAuthentication();

    builder.Services.ConfigureApplicationCookie(o =>
    {
        o.Cookie.HttpOnly = true;
        o.Cookie.SameSite = SameSiteMode.Strict;
        o.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        o.ExpireTimeSpan = TimeSpan.FromDays(14);
        o.SlidingExpiration = true;

        o.LoginPath = "/login";
        o.LogoutPath = "/api/v1/auth/logout";

        o.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };
    });

    builder.Services.AddAuthorization();
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("VueFrontend", p =>
        {
            p.WithOrigins("http://localhost:5174")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
    });
    
    builder.Services.AddFastEndpoints();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwaggerUi();
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
    });

    app.UseSwaggerGen();

    await using (var scope = app.Services.CreateAsyncScope())
    {
        var services = scope.ServiceProvider;
        var dbContext = services.GetRequiredService<ApplicationDbContext>();

        await dbContext.Database.MigrateAsync();

        await IdentitySeeder.SeedRolesAndAdminAsync(services, app.Configuration);
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