using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using WebAPI.Core.handlers;
using WebAPI.Demo.permission;
using WebAPI.Features.AuthAPI.handler.permission;

namespace WebAPI.Core.configs;

public class SecurityConfig
{
    public static void Configure(WebApplicationBuilder builder)
    {   
        // -- Policy config --
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("CanRead", policy => 
                policy.Requirements.Add(new PermissionRequirement("Read"))
            );
            options.AddPolicy("CanExport", policy => 
                policy.Requirements.Add(new PermissionRequirement("Export"))
            );
        });
        // -- Config openapi security schema -- 
        builder.Services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
        });
        builder.Services.AddOpenApi();
        // -- Config auth service -- 
        builder.Services.AddAuthentication(opts =>
        {
            opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(opts =>
        {
            String secretKey = builder.Configuration["Jwt:secretKey"] ?? "";
            String issuer = builder.Configuration["Jwt:issuer"] ?? "";
            String audience = builder.Configuration["Jwt:audience"] ?? "";
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
        builder.Services.AddAuthentication("Cookies").AddCookie("Cookies");
        // -- Config identity -- 
        builder.Services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequiredLength = 3;
            options.Password.RequiredUniqueChars = 1;
        });
    }
}