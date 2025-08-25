using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using WebAPI.Core;
using WebAPI.Example;
using WebAPI.Features.Account;
using WebAPI.Features.Shared;

namespace WebAPI.Features.Auth;

[ApiController]
[Route("/api/v1/")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly ILogger<AuthController> _logger;
    
    public AuthController(
        AuthService authService,
        ILogger<AuthController> logger
        )
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("auth/login")]
    public ActionResult<object> loginAccountAPI(
        [FromBody] AccountDTO req
    )
    {
        Dictionary<String, Object> result = _authService.createToken(req);
        var token = CustomJson.json(result, CustomJsonOptions.None);
        HttpContext.Response.Cookies.Append("token", token, new CookieOptions
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
        
        _logger.LogInformation(CustomJson.json(response, CustomJsonOptions.WriteIndented));
        
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
    
    [HttpPost("refresh")]
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

    [HttpGet("auth/testAdmin")]
    // [TypeFilter(typeof(JwtFilter))]
    [Authorize(Roles = "Admin")]
    public ActionResult<Object> AdminDashboard()
    {
        return "DashBoard";
    }

    [HttpGet("auth/testUser")]
    // [TypeFilter(typeof(JwtFilter))]
    [Authorize(Roles = "User")]
    public ActionResult<Object> userOptions()
    {
        return ":D";
    }
}
