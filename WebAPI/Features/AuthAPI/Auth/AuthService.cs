using Microsoft.AspNetCore.Identity;
using WebAPI.Annotation;
using WebAPI.Core.shared;
using WebAPI.Core.utils;
using WebAPI.Entities;
using WebAPI.Features.AccAPI;
using WebAPI.Features.AuthAPI.RefreshToken;
using WebAPI.Features.AuthAPI.service;

namespace WebAPI.Features.AuthAPI.Auth;

[Service]
public class AuthService
{
    private readonly JwtTokenProvider _tokenProvider;
    private readonly AccountRepository _accountRepository;
    private readonly RefreshTokenRepository _refreshTokenRepository;
    private readonly Pbkdf2PasswordHasher _pbkdf2PasswordHasher;

    public AuthService(
        JwtTokenProvider tokenProvider,
        AccountRepository accountRepository,
        RefreshTokenRepository refreshTokenRepository,
        Pbkdf2PasswordHasher pbkdf2PasswordHasher
    )
    {
        _tokenProvider = tokenProvider;
        _accountRepository = accountRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _pbkdf2PasswordHasher = pbkdf2PasswordHasher;
    }

    public Account registerAccount(AccountDTO account)
    {
        Account acc = new Account();
        acc.Id = Guid.NewGuid().ToString();
        acc.RoleId = "2";
        acc.Username = account.username;
        acc.PasswordHash = _pbkdf2PasswordHasher.hashPassword(account.password);
        Console.WriteLine(CustomJson.json(acc, CustomJsonOptions.None));
        
        var affectedRows = _accountRepository.add(acc);
        if(affectedRows < 0) throw new ApplicationException("Failed to register account");
        
        return acc;
    }

    public async Task<Dictionary<String, Object>> createToken(AccountDTO req)
    {
        var user = (await _accountRepository.findByUsernameAndRole(req.username)).First() ?? throw new ApplicationException("Invalid username");
        Console.WriteLine(CustomJson.json(user, CustomJsonOptions.WriteIndented));
        Boolean checkPassword = _pbkdf2PasswordHasher.verifyPassword(req.password, user["passwordHash"].ToString());
        
        if(!checkPassword) throw new ApplicationException("Invalid password");
        
        String accessToken = await _tokenProvider.generateAcessToken(user);
        String refreshToken = _tokenProvider.generateRefreshToken(user);
        
        return await toDict(accessToken, refreshToken);
    }

    public async Task<Dictionary<String, Object>> refreshToken(RefreshTokenDTO dto)
    {
        var rt = _refreshTokenRepository.findByToken(dto.token);
        
        if (TimeUtils.AsiaTimeZone(DateTime.UtcNow) > rt.ExpiryDate) throw new Exception("Expired token is expired");

        var user = _accountRepository.findById(rt.AccountId);
        
        String accessToken = await _tokenProvider.generateAcessToken(user);

        return await toDict(accessToken, accessToken);
    }
    
    public async Task<Dictionary<String, Object>> toDict(String accessToken, String refreshToken)
    {
        return new Dictionary<string, object>
        {
            ["access-token"] = accessToken,
            ["refresh-token"] = refreshToken
        };
    }
}
