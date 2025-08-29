using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using WebAPI.Annotation;
using WebAPI.Core.handlers;
using WebAPI.Core.shared;
using WebAPI.Core.utils;
using WebAPI.Features.AccAPI;
using WebAPI.Features.AuthAPI.RefreshToken;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace WebAPI.Features.AuthAPI.Auth;

[Component]
public class JwtTokenProvider
{
    private readonly String _key;
    private readonly String _issuer;
    private readonly String _audience;
    private readonly String _atExpireTime; 
    private readonly String _rtExpireTime; 
    private readonly RefreshTokenRepository _refreshTokenRepository;
    private readonly AccountRepository _accountRepository;
    
    public JwtTokenProvider(
        IConfiguration config,
        RefreshTokenRepository refreshTokenRepository,
        AccountRepository accountRepository
        )
    {
        var atTime = new DataTable().Compute(config["Jwt:atExpiryInMillisecond"], null).ToString();
        var rtTime = new DataTable().Compute(config["Jwt:rtExpiryInMillisecond"], null).ToString();
        
        _key = config["Jwt:secretKey"] ?? "";
        _issuer = config["Jwt:issuer"] ?? "";
        _audience = config["Jwt:audience"] ?? "";
        _atExpireTime = atTime ?? "";
        _rtExpireTime = rtTime ?? "";
        _refreshTokenRepository = refreshTokenRepository;
        _accountRepository = accountRepository;
    }

    public async Task<String> generateAcessToken<TUser>(TUser user) where TUser : class
    {
        var dictUser = ConverterUtils.toDict(user);
        Console.WriteLine($"GenToken: {CustomJson.json(user, CustomJsonOptions.WriteIndented)}");

        var permissions = (await _accountRepository.findByUsernameAndPermission(dictUser["username"].ToString())).Select(x => x["name"]);
        
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, dictUser["id"].ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("role", dictUser["roleName"].ToString())
        };
        
        foreach (var permission in permissions) {
            claims.Add(new Claim("permission", permission.ToString()));
        }
        
        string secretKey = _key;
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        
        var tokenDes = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMilliseconds(Convert.ToInt32(_atExpireTime)),
            signingCredentials: credentials
        );

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.WriteToken(tokenDes);
        
        return jwtToken;
    }

    public String generateRefreshToken<TUser>(TUser user) where TUser : class
    {
        var u = _accountRepository.findByUsername(getValue(user, "username")?.ToString()!);

        var dt = DateTime.UtcNow.AddMilliseconds(Convert.ToInt32(_rtExpireTime));
        
        Entities.RefreshToken rt = new Entities.RefreshToken();
        rt.Id = Guid.NewGuid().ToString();
        rt.Token = Guid.NewGuid().ToString();
        rt.AccountId = u?.Id;
        rt.ExpiryDate = TimeUtils.AsiaTimeZone(dt);
        
        _refreshTokenRepository.add(rt);
        
        return rt.Token;
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
                string.Equals(p.Name, propertyName, StringComparison.OrdinalIgnoreCase)
            );
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
