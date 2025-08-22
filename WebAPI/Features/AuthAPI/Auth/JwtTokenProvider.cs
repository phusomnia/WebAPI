using Microsoft.AspNetCore.Identity;
using WebAPI.Annotation;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using WebAPI.Entities;
using WebAPI.Features.Account;
using WebAPI.Features.AuthService.RefreshToken;

namespace WebAPI.Features.Auth;

[Component]
public class JwtTokenProvider
{
    private readonly string _key;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly string _expireTime; 
    private readonly RefreshTokenRepository _refreshTokenRepository;
    private readonly AccountRepository _accountRepository;
    
    public JwtTokenProvider(
        IConfiguration config,
        RefreshTokenRepository refreshTokenRepository,
        AccountRepository accountRepository
        )
    {
        _key = config["Jwt:SecretKey"] ?? "";
        _issuer = config["Jwt:Issuer"] ?? "";
        _audience = config["Jwt:Audience"] ?? "";
        _expireTime = config["Jwt:ExpiryInSecond"] ?? "";
        _refreshTokenRepository = refreshTokenRepository;
        _accountRepository = accountRepository;
    }

    public string generateAcessToken<TUser>(TUser user) where TUser : class
    {
        Console.WriteLine(user.ToString());
        Console.WriteLine(Convert.ToInt32(_expireTime));

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, getValue(user, "id")?.ToString()!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, getValue(user, "roles")?.ToString()!)
        };
        
        string secretKey = _key;
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        
        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMilliseconds(Convert.ToInt32(_expireTime)),
            signingCredentials: credentials
        );
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string generateRefreshToken<TUser>(TUser user) where TUser : class
    {
        var u = _accountRepository.findByUsername(getValue(user, "username")?.ToString()!);
        
        RefreshToken rt = new RefreshToken();
        rt.Id = Guid.NewGuid().ToString();
        rt.Token = Guid.NewGuid().ToString();
        rt.AccountId = u?.Id;
        rt.ExpiryDate = DateTime.Now;

        _refreshTokenRepository.Add(rt);
        
        return Guid.NewGuid().ToString();
    }

    public JwtSecurityToken? extractAllClaims(String token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        return jwtToken;
    }

    public JwtPayload? extractPayload(string token)
    {
        return extractAllClaims(token)!.Payload;
    }

    public Object? getValue<TUser>(TUser obj, string propertyName) where TUser : class
    {
        var prop = typeof(TUser).GetProperties()
            .FirstOrDefault(p => 
                string.Equals(p.Name, propertyName, StringComparison.OrdinalIgnoreCase
                ));
        return prop?.GetValue(obj) ?? "";
    }

    public ClaimsPrincipal? validateToken(String token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        
        return tokenHandler.ValidateToken(token, new TokenValidationParameters
        {
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _issuer,
            ValidAudience = _audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_key)),
            ClockSkew = TimeSpan.Zero
        }, out _);
    }
}
