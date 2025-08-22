using System.Reflection;
using System.Security.Claims;
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
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using StackExchange.Redis;
using WebAPI;
using WebAPI.Config;
using WebAPI.Entities;
using WebAPI.Example;
using WebAPI.Example.Cache;
using WebAPI.Features.Cache;
using WebAPI.Features.RealTime;
using WebAPI.Middlewares;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.AddAnnotation(Assembly.GetExecutingAssembly());

// ???
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// Load only the development appsettings
builder.Configuration.AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true);

// Config name convention and json serializer
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// Config allow snakecase
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
    options.SerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.SnakeCaseLower;
});

// Add identity
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
        options.SignIn.RequireConfirmedAccount = true
).AddEntityFrameworkStores<AppDbContext>();

// Config openapi security schema
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});
builder.Services.AddOpenApi();

// Scope of service
builder.Services.AddScoped<PasswordHasher<Account>>();

// Config database context
var defaultConnection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseMySql(
    defaultConnection,
    ServerVersion.AutoDetect(defaultConnection)
));
builder.Services.AddDbContext<CustomIdentityDbContext>(options => options.UseMySql(
    defaultConnection,
    ServerVersion.AutoDetect(defaultConnection)
));

// Config cache
// Register Redis connection
builder.Services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(builder.Configuration.GetValue<string>("Redis")));

// Config auth service
builder.Services.AddAuthentication(opts =>
{
    opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(opts =>
{
    String secretKey = builder.Configuration["Jwt:SecretKey"] ?? "";
    String issuer = builder.Configuration["Jwt:Issuer"] ?? "";
    String audience = builder.Configuration["Jwt:Audience"] ?? "";
    opts.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero
    };
});

// Config security services
builder.Services.AddAuthentication("Cookies").AddCookie("Cookies");
builder.Services.AddAuthorization(options =>
{
    // options.AddPolicy("Admin", policy => policy.RequireClaim("CTO").RequireRole("Manager"));
});

// builder.Services.AddControllers(otps => otps.Filters.Add<JwtFilter>());

// Config signalR
builder.Services.AddSignalR();

void ExampleService()
{
    builder.Services.AddScoped<CacheResourceFilter>();
    builder.Services.AddScoped<CacheManager>();
    builder.Services.AddControllers(opts =>
    {
        opts.Filters.Add<CacheResourceFilter>();
    });
    builder.Services.AddMemoryCache();
}
// ExampleService();

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
    // app.UseHsts();
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
app.MapGet("/checkConnection", (AppDbContext ctx1, CustomIdentityDbContext ctx2) =>
{
    var canConnect1 = ctx1.Database.CanConnect();
    var canConnect2 = ctx2.Database.CanConnect();
    return new Dictionary<String, dynamic>
    {
        ["App"] = canConnect1 ? Results.Ok("Ok") : Results.BadRequest("Can't connect to database"),
        ["Identity"] = canConnect2 ? Results.Ok("Ok") : Results.BadRequest("Can't connect to database"),
    };
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
    // ctx.Response.Cookies.Append("auth", "admin", new CookieOptions
    // {
    //     HttpOnly = true
    // });

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
