using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.WebUtilities;
using WebAPI.Core.shared;

namespace WebAPI.Example.Identity;


[ApiController]
[Route("/api/v1/")]
public class IdentityController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IEmailSender _emailSender;
    
    public IdentityController(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        RoleManager<IdentityRole> roleManager,
        IEmailSender emailSender
        )
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _emailSender = emailSender;
    }
    
    [AllowAnonymous]
    [HttpPost("register-demo")]
    public async Task<IActionResult> register(RegisterDTO dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        
        var user = new IdentityUser { UserName = dto.username, Email = dto.email };
        var result = await _userManager.CreateAsync(user, dto.password);
        if (!result.Succeeded)
            return BadRequest(result.Errors);
        
        // var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var payload = new { userId = user.Id, token = token };
        var encodedPayload = WebEncoders.Base64UrlEncode(JsonSerializer.SerializeToUtf8Bytes(payload));
        Console.WriteLine("TOKEN REGISTER: " + CustomJson.json(token, CustomJsonOptions.None));
        
        var confirmationURL = Url.Action(
            nameof(confirmEmail),
            "Identity",
            new { payload = encodedPayload },
            Request.Scheme
            ) ?? throw new Exception("Can't create url");
        
        await _emailSender.SendEmailAsync(
            user.Email,
            "Confirm your email",
            $"Please confirm your account by <a href={HtmlEncoder.Default.Encode(confirmationURL)}>Link<a/>."
        );
        
        return Ok("User registered");
    }
    
    [AllowAnonymous]
    [HttpGet("confirm-email-demo")]
    public async Task<IActionResult> confirmEmail(
        [FromQuery] String payload
        )
    {   
        try
        {
            var decodedPayload = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(payload));
            var jsonData = JsonSerializer.Deserialize<Dictionary<String, Object>>(decodedPayload);
            
            var user = await _userManager.FindByIdAsync(jsonData["userId"].ToString());
            if (user == null) return NotFound("User not found");
            
            if (user.EmailConfirmed) return BadRequest("Email has been activated");
            
            var result = await _userManager.ConfirmEmailAsync(user, jsonData["token"].ToString());
            if (!result.Succeeded) return BadRequest(result.Errors);
            
            return Ok("Email confirmed successfully");
        }
        catch (Exception e)
        {
            return StatusCode(500, new { message = e.Message });
        }
    }
    
    [HttpPost("login-demo")]
    public IActionResult login([FromBody] LoginDTO dto)
    {
        Console.WriteLine(CustomJson.json(dto, CustomJsonOptions.None));
        // var result = _signInManager.PasswordSignInAsync(dto.username, dto.password, false, false);
        // if (!result.Result.Succeeded)
        //     return Unauthorized();

        return Ok("Login successfully");
    }

    [HttpGet("profile-demo")]
    [Authorize]
    public IActionResult profile()
    {
        return Ok(new { User = User.Identity?.Name });
    }

    [AllowAnonymous]
    [HttpPost("send-forget-password-demo")]
    public async Task<IActionResult> sendEmail([FromBody] ForgetPasswordDTO dto)
    {
        try
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            
            var user = await _userManager.FindByEmailAsync(dto.email);
            if (user == null) return NotFound("User not found");
            
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var payload = new { userId = user.Id, token = token, password = dto.password };
            var encodedPayload = WebEncoders.Base64UrlEncode(JsonSerializer.SerializeToUtf8Bytes(payload));
            
            // manual
            // var callbackUrl = $"{Request.Scheme}://{Request.Host}/api/v1/{nameof(resetPassword)}?payload={encodedPayload}";
            
            var callbackUrl = Url.Action(
                nameof(resetPassword),
                "Identity",
                new { payload = encodedPayload },
                Request.Scheme
                ) ?? throw new Exception("Can't create url");
            
            await _emailSender.SendEmailAsync(
                user.Email,
                "Reset Password",
                $"Please reset your password by clicking here: <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>link</a>");

            return Ok("Password reset link has been sent to your email");
        }
        catch (Exception e)
        {
            return StatusCode(500, new { message = e.Message });
        }
    }
    
    [AllowAnonymous]
    [HttpGet("reset-password-demo")]
    public async Task<IActionResult> resetPassword([FromQuery] ResetPasswordDTO dto)
    {
        try
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            
            var decodedPayload = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(dto.payload));
            var payload = JsonSerializer.Deserialize<Dictionary<String, Object>>(decodedPayload)!;
            
            Console.WriteLine("LOG: \n" + CustomJson.json(payload, CustomJsonOptions.WriteIndented));
            
            var user = await _userManager.FindByIdAsync(payload["userId"].ToString());
            if (user == null) return NotFound("User not found");
            
            var result = await _userManager.ResetPasswordAsync(user, payload["token"].ToString(), payload["password"].ToString());
            
            if (!result.Succeeded) return BadRequest(result.Errors);
            
            return Ok(new { message = "Password has been reset successfully" });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { message = e.Message });
        }
    }
}

public record RegisterDTO(String username,String email,String password);
public record LoginDTO(String username,String password);
public record SendEmailDTO(String To, String Subject, String Body);
public record ForgetPasswordDTO(String email, String password);
public record ConfirmEmailDTO(String userId, String token);
public record ResetPasswordDTO(String payload);
