using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace WebAPI.Core.configs;

internal sealed class BearerSecuritySchemeTransformer(IAuthenticationSchemeProvider authenticationSchemeProvider) : IOpenApiDocumentTransformer
{
    public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        var requirements = new Dictionary<string, OpenApiSecurityScheme>
        {
            ["Bearer"] = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer", 
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