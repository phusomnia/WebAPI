using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using WebAPI.Context;
using Microsoft.EntityFrameworkCore;
using WebAPI.Annotation;
using WebAPI.Core;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddAnnotation(Assembly.GetExecutingAssembly());
// TODO: Build Logger
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);
// Load only the development appsettings
builder.Configuration.AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true);
// [SERVICES]
// Ignore null 
builder.Services.ConfigureHttpJsonOptions(options => 
{
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
// allow snakecase 
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
    options.SerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.SnakeCaseLower;
});
// 
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddControllers();
// config openapi security scheme
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});
builder.Services.AddOpenApi();
// database context
var defaultConnection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseMySql(
    defaultConnection,
    ServerVersion.AutoDetect(defaultConnection)
));
var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}
// [SECURITY]
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
// [ROUTING]
app.MapControllers();
// TODO: Config JWT
// TODO: Config dyanmic DI 
// TODO: Config dynamic endpoint
// redirect at / -> scalar/v1
app.MapGet("/", () => Results.Redirect("/scalar/v1")).ExcludeFromDescription();
// check connection if default string is valid
app.MapGet("/checkConnection", (AppDbContext ctx) =>
{
    var canConnect = ctx.Database.CanConnect();
    return canConnect ? Results.Ok("Ok") : Results.BadRequest("Can't connect to database");
});
app.MapPost("/test", ([FromQuery] BaseStatus res) =>
{
    APIResponse<dynamic> response = new APIResponse<dynamic>(
        res,
        null,
        "1"
    );
    return Results.Ok(response);
});
app.Run();
// [SECURITY SCHEME]
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