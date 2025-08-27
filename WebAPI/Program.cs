using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Scalar.AspNetCore;
using WebAPI.Context;
using WebAPI.Annotation;
using WebAPI.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using WebAlvPI.Example.Validate;
using WebAPI;
using WebAPI.Config;
using WebAPI.Entities;
using WebAPI.Example;
using WebAPI.Example.Cache;
using WebAPI.Example.Identity;
using WebAPI.Features.RealTime;
using WebAPI.Features.Shared;
using WebAPI.Middlewares;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.AddAnnotation(Assembly.GetExecutingAssembly());

// Load only the development appsettings
builder.Configuration.AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true);

LoggingConfig.configure(builder);
NamingConventionConfig.configure(builder);
DatabaseConfig.configure(builder);
// builder.Services.AddHttpContextAccessor();
// builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
builder.Services.AddTransient<IEmailSender, EmailSender>();
SecurityConfig.Configure(builder);
ExampleConfig.configure(builder);
RealTimeConfig.Configure(builder);

builder.Services
    .AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = CustomInvalidModelState.BuildResponse;
    });

var app = builder.Build();

// Default exception handling middleware
app.UseCustomExceptionMiddleware();

// Example: inline middleware
void Example()
{
    app.Use(async (ctx, next) =>
    {
        Console.WriteLine("Incoming request: " + ctx.Request.Path);
        await next();  // Pass to the next middleware
        Console.WriteLine("Outgoing response status: " + ctx.Response.StatusCode);
    });
    app.UseExampleMiddleware();
}
// Example();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapHub<ChatHub>("/chatHub");

// Add controllers
app.MapControllers();
app.MapGet("/", () => Results.Ok("Ok"))
    .ExcludeFromDescription();

app.MapGet("/checkConnection", (
    ILogger<Program> logger,
    AppDbContext adc,
    CustomIdentityDbContext ctx2
    ) =>
{
    var response = new Dictionary<String , Object>() 
    {
        ["App"] = adc.Database.CanConnect() ? Results.Ok("Ok") : Results.BadRequest("Can't connect to database"),
        ["Identity"] = ctx2.Database.CanConnect() ? Results.Ok("Ok") : Results.BadRequest("Can't connect to database"),
    };
    
    logger.LogInformation(CustomJson.json(response, CustomJsonOptions.WriteIndented));
    
    return response;
});
app.MapPost("/test", ([FromQuery] BaseStatus res) =>
{
    Console.WriteLine(builder.Configuration["Jwt:Key"]);
    APIResponse<dynamic> response = new APIResponse<dynamic>(
        res,
        null,
        "1"
    );
    return Results.Ok(response);
}).AddEndpointFilter<AEndpointFilter>()
.AddEndpointFilter<BEndpointFilter>()
.AddEndpointFilter<CEndpointFilter>();

app.MapGet("/set-cookie", async (HttpContext ctx) =>
{
    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, "Phusomnia"),
        new Claim(ClaimTypes.Role, "User")
    };

    var identity = new ClaimsIdentity(claims, "Cookies");
    var principal = new ClaimsPrincipal(identity);

    await ctx.SignInAsync("Cookies", principal);

    Console.WriteLine("OK");
    return Results.Ok("Logged in with set cookie");
});

// Run application
app.Run();
