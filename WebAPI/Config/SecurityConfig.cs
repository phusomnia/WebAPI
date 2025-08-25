using System.Text;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using WebAPI.Config;
using WebAPI.Context;

namespace WebAPI;

public class SecurityConfig
{
    public static void Configure(WebApplicationBuilder builder)
    {
        builder.Services.AddDefaultIdentity<IdentityUser>(options =>
            options.SignIn.RequireConfirmedAccount = true
        ).AddEntityFrameworkStores<AppDbContext>();

        // Config openapi security schema
        builder.Services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
        });
        builder.Services.AddOpenApi();
        
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
    }
}