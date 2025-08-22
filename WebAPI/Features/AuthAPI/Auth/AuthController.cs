using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using WebAPI.Core;
using WebAPI.Example;
using WebAPI.Features.Account;

namespace WebAPI.Features.Auth;

[ApiController]
[Route("/api/v1/")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("auth/login")]
    public ActionResult<object> loginAccountAPI(
        [FromBody] AccountDTO req
    )
    {
        Dictionary<String, Object> result = _authService.createToken(req);
        String token = (String)result["access-token"];
        
        HttpContext.Response.Cookies.Append("jwt", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict
        });
        
        APIResponse<object> response = new APIResponse<dynamic>(
            BaseStatus.Success,
            "Login",
            result
        );
        
        return response;
    }

    [HttpPost("register")]
    public ActionResult<object> registerAccountAPI(
        [FromBody] AccountDTO req
    )
    {
        var result = _authService.registerAccount(req);
        
        var response = new APIResponse<object>(
            BaseStatus.Success,
            "Login",
            result
        );

        return response;
    }
    
    [HttpPost("refresh-token")]
    public ActionResult<Object> refreshTokenAPI(
        [FromBody] RefreshTokenDTO token
        )
    {
        var result = _authService.refresh(token);
        
        var response = new APIResponse<Object>(
            BaseStatus.Success,
            "Refresh",
            result
        );

        return response;
    }

    [HttpGet("auth/dashboard")]
    [TypeFilter(typeof(JwtFilter))]
    public ActionResult<Object> AdminDashboard()
    {
        return "DashBoard";
    }

    [HttpPost("auth")]
    [Authorize(Roles = "User")]
    public ActionResult<dynamic> testAuthAPI()
    {
        return ":D";
    }
}
