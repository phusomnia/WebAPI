using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebAPI.Features.AuthAPI.Auth;

namespace WebAPI.Core.filters;

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
        var principal = _provider.validateToken(token);
        ctx.HttpContext.User = principal;
    }
}