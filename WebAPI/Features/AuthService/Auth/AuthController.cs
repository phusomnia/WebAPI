using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using WebAPI.Core;
using WebAPI.Features.Account;

namespace WebAPI.Features.Auth;

[ApiController]
[Route("/api/v1/[controller]")]
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
        return new APIResponse<object>(
            BaseStatus.Success,
            "Login",
            "result"
        );
    }
    [HttpPost("refresh-token")]
    public ActionResult<object> refreshTokenAPI(
        [FromBody] RefreshTokenDTO token
        )
    {
        var result = _authService.refresh(token);
        return new APIResponse<object>(
            BaseStatus.Success,
            "Refresh",
            result
        );
    }
    [HttpPost("auth")]
    [Authorize(Roles = "User")]
    public ActionResult<dynamic> testAuthAPI()
    {
        return ":D";
    }
}
