using Ala.Backend.Application;
using Ala.Backend.Infrastructure;
using Ala.Backend.Persistence;
using Ala.Backend.WebAPI;
using Ala.Backend.WebAPI.Extensions;
using Ala.Backend.WebAPI.Middlewares;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.AddSerilogConfiguration();

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddPresentationServices(builder.Configuration);

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddOpenApi();

var app = builder.Build();
app.UseSerilogRequestLogging();
// Configure the HTTP request pipeline.
app.MapOpenApi(); // Arka planda OpenAPI JSON dosyasýný üretir

app.MapScalarApiReference(options =>
{
    options.WithTitle("Ala Backend API")
           .WithTheme(ScalarTheme.BluePlanet) // Purple temasý varsayýlan olarak açýk býrakýldý
           .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
});



app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseHttpsRedirection();

app.UseAuthentication();  
app.UseAuthorization();

app.MapControllers();

app.Run();
