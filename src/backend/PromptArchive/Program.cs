using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PromptArchive.Database;
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

    builder.Services.AddFastEndpoints();

    builder.Services.AddSwaggerDocument()
        .AddEndpointsApiExplorer();

    builder.Services.AddDbContext<ApplicationDbContext>(c =>
        c.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

    builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

    builder.Services.AddAuthentication();
    builder.Services.AddAuthorization();

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

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwaggerUi();
        app.UseDeveloperExceptionPage();
    }

    app.UseCors("VueFrontend");
    app.UseHttpsRedirection();
    app.UseAuthorization();

    app.UseFastEndpoints(c =>
    {
        c.Versioning.Prefix = "v";
        c.Versioning.DefaultVersion = 1;
        c.Versioning.PrependToRoute = true;
        c.Endpoints.RoutePrefix = "api";
    });

    app.UseSwaggerGen();

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