using Serilog;
using saas_template.Extensions;
using saas_template.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog logging
builder.AddSerilogLogging();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

// Add application services (database, repositories, services)
builder.Services.AddApplicationServices(builder.Configuration);

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add rate limiting
builder.Services.AddRateLimiting(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Request logging middleware (first - to capture all requests)
app.UseMiddleware<RequestLoggingMiddleware>();

// Rate limiting
app.UseRateLimiter();

app.UseHttpsRedirection();

app.UseCors();

// Global exception handling middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthorization();

app.MapControllers();

// Health check endpoints
app.MapHealthChecks("/health");
app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});
app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = _ => false
});

try
{
    app.Run();
}
finally
{
    Log.CloseAndFlush();
}

// Make Program class accessible for integration tests
namespace saas_template;

public partial class Program { }
