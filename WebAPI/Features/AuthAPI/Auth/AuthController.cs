using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Core;
using WebAPI.core.dto;
using WebAPI.Core.filters;
using WebAPI.Core.shared;
using WebAPI.Demo.permission;
using WebAPI.Features.AccAPI;

namespace WebAPI.Features.AuthAPI.Auth;

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
    public async Task<IActionResult> loginAccountAPI(
        [FromBody] AccountDTO req
    )
    {
        var result = await _authService.createToken(req);
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
        
        return Ok(response);
    }

    [HttpPost("register")]
    public async Task<IActionResult> registerAccountAPI(
        [FromBody] AccountDTO req
    )
    {
        var result = _authService.registerAccount(req);
        
        var response = new APIResponse<Object>(
            BaseStatus.Success,
            "Login",
            result
        );

        return Ok(response);
    }
    
    [HttpPost("refresh")]
    public async Task<IActionResult> refreshTokenAPI(
        [FromBody] RefreshTokenDTO token
        )
    {
        var result = _authService.refreshToken(token);
        
        var response = new APIResponse<Object>(
            BaseStatus.Success,
            "Refresh",
            result
        );

        return Ok(response);
    }

    [HttpGet("auth/secure-by-role")]
    // [Authorize(Roles = "User")]
    [Authorize(Roles = "Admin")]
    public ActionResult<String> AdminDashboard()
    {
        return Redirect("http://localhost:3000/api/v1/auth/secure-by-permission");
    }
    
    [HttpGet("auth/secure-by-permission")]
    [Permission("Export")]
    public ActionResult<Object> exportPDF()
    {
        return "PDF is ok";
    }
}
