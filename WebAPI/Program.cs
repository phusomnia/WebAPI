using System.Diagnostics;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

// TODO: Build Logger
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// Load only the development appsettings
builder.Configuration.AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true);

// [SERVICES]
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddControllers();
// TODO: Config Bearer
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});
builder.Services.AddOpenApi();
// TODO: implement db service


var app = builder.Build();

// Get the logger
// var log = app.Services.GetRequiredService<ILogger<Program>>();
// log.LogInformation("Starting web host");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}


app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// TODO: Config JWT
// TODO: Config dyanmic DI 
// TODO: Config dynamic endpoint

app.MapGet("/", () => "Hello World!");
// Todo: Check connection from db
app.MapGet("/checkConnection", () =>
{
    return "abc";
});
app.MapGet("/testConfig", () =>
{
    return builder.Configuration["Test"];
});

app.Run();

// Todo: Security Scheme
internal sealed class BearerSecuritySchemeTransformer(IAuthenticationSchemeProvider authenticationSchemeProvider) : IOpenApiDocumentTransformer
{
    public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        // Define the Bearer scheme in OpenAPI
        var requirements = new Dictionary<string, OpenApiSecurityScheme>
        {
            ["Bearer"] = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer", // "bearer" refers to the header name here
                In = ParameterLocation.Header,
                BearerFormat = "Json Web Token"
            }
        };
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes = requirements;
        
        var authenticationSchemes = await authenticationSchemeProvider.GetAllSchemesAsync();
        if (authenticationSchemes.Any(authScheme =>
                string.Equals(authScheme.Name, "Bearer", StringComparison.OrdinalIgnoreCase)))
        {
            document.SecurityRequirements ??= new List<OpenApiSecurityRequirement>();
            document.SecurityRequirements.Add(new OpenApiSecurityRequirement
            {
                {
                    document.Components.SecuritySchemes["Bearer"],
                    Array.Empty<string>()
                }
            });
        }
    }
}