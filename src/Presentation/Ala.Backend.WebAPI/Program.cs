using Ala.Backend.Application;
using Ala.Backend.Infrastructure;
using Ala.Backend.Persistence;
using Ala.Backend.WebAPI;
using Ala.Backend.WebAPI.Extensions;
using Ala.Backend.WebAPI.Middlewares;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog Configuration
builder.Host.AddSerilogConfiguration();

// Services
builder.Services.AddControllers();

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddPresentationServices(builder.Configuration);

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

builder.Services.AddOpenApi();

var app = builder.Build();

//
// MIDDLEWARE PIPELINE
//

// CorrelationId üretimi (tüm request boyunca kullanýlacak)
app.UseMiddleware<CorrelationIdMiddleware>();

// HTTP request logging (Serilog)
app.UseSerilogRequestLogging();

// Global exception handling
app.UseMiddleware<GlobalExceptionMiddleware>();

// HTTPS redirect
app.UseHttpsRedirection();

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();


// OpenAPI JSON
app.MapOpenApi();

// Scalar API UI
app.MapScalarApiReference(options =>
{
    options.WithTitle("Ala Backend API")
           .WithTheme(ScalarTheme.BluePlanet)
           .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
});

// Controllers
app.MapControllers();

app.Run();