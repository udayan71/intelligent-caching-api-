using Application.Interfaces.Services;
using Infrastructure.DependencyInjection;
using intelligent_caching_api.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Disambiguate the AddInfrastructure call by specifying the class name explicitly.
// Choose the correct one based on your intent. Here, using ServiceRegistration as an example.
// If you want ServiceCollectionExtensions, replace ServiceRegistration with ServiceCollectionExtensions.
ServiceRegistration.AddInfrastructure(builder.Services, builder.Configuration);

var app = builder.Build();

// keep HTTPS redirection early
app.UseHttpsRedirection();

// Serve Swagger UI unconditionally for debugging and make the endpoint explicit.
// Remove or wrap this in an environment check for production.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    // Ensure the JSON endpoint path is correct for your app
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Intelligent Caching API v1");

    // Option A: serve swagger at the default /swagger (explicit)
    c.RoutePrefix = "swagger";

    // Option B: serve swagger at the app root (https://localhost:7149/)
    //c.RoutePrefix = string.Empty;
});

app.UseMiddleware<ResponseTimeMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();