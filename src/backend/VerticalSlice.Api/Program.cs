using System.Text.Json.Serialization;
using VerticalSlice.Api.Authentication;
using VerticalSlice.Api.Configuration;
using VerticalSlice.Api.Endpoints;
using VerticalSlice.Api.Extensions;
using VerticalSlice.Api.Logging;
using VerticalSlice.Api.Middleware;
using VerticalSlice.Infrastructure;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ApiExceptionHandler>();
builder.Services.AddApiAuthentication(builder.Configuration);
builder.Services.AddApiConfiguration(builder.Configuration);
builder.Services.AddApiFeatureManagement();
builder.Services.AddApiRateLimiting(builder.Configuration);
builder.Services.AddValidatorsFromAssemblyContaining<Program>(
    lifetime: ServiceLifetime.Singleton,
    includeInternalTypes: true);
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<StructuredRequestLoggingMiddleware>();
app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseForwardedHeaders();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseRateLimiter();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapDefaultEndpoints();
app.MapEndpoints();

app.Run();

public partial class Program;