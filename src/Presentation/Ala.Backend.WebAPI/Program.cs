using Ala.Backend.Application;
using Ala.Backend.Application.Abstractions.Persistence.Seeders;
using Ala.Backend.Infrastructure;
using Ala.Backend.Persistence;
using Ala.Backend.WebAPI;
using Ala.Backend.WebAPI.Extensions;
using Ala.Backend.WebAPI.Middlewares;
using Ala.Backend.WebAPI.Startup;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.AddSerilogConfiguration();
builder.Services.AddMemoryCache();

// Services
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddPresentationServices(builder.Configuration);

builder.Services.AddOpenApi();


var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.MapOpenApi();

    app.MapScalarApiReference(options =>
    {
        options.WithTitle("EuroScada API")
               .WithTheme(ScalarTheme.BluePlanet)
               .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<IPermissionSeeder>();
    var permissions = PermissionScanner.ScanControllers(typeof(Program).Assembly);
    await seeder.SyncPermissionsAsync(permissions);
}


await app.RunAsync();



