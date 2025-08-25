using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using WebAPI.Annotation;
using WebAPI.Features.Auth;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace WebAPI.Example;

public class JwtFilter : IAsyncAuthorizationFilter
{
    private readonly JwtTokenProvider _provider;
    private readonly ILogger<JwtFilter> _logger;

    public JwtFilter(
        JwtTokenProvider provider,
        ILogger<JwtFilter> logger
        )
    {
        _provider = provider;
        _logger = logger;
    }
    
    public async Task OnAuthorizationAsync(AuthorizationFilterContext ctx)
    {
        var authHeader = ctx.HttpContext.Request.Headers["Authorization"];
        _logger.LogInformation($"Auth header: {authHeader}");
        
        if (String.IsNullOrEmpty(authHeader))
        {
            ctx.Result = new JsonResult(new
            {
                status = "error",
                message = "Auth header is null or invalid"
            })
            {
                StatusCode = 403
            };
            return;
        }

        String token = authHeader.ToString().Substring(7);
        var principal = _provider.validateToken(token!)!;
        ctx.HttpContext.User = principal;

        // _logger.LogInformation(principal.Claims.FirstOrDefault(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value);
        // if (!principal.IsInRole("Admin"))
        // {
        //     ctx.Result = new UnauthorizedResult();
        // }
    }
}