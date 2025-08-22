using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using WebAPI.Annotation;
using WebAPI.Entities;
using WebAPI.Features.Account;
using WebAPI.Features.AuthService.RefreshToken;

namespace WebAPI.Features.Auth;

[Service]
public class AuthService
{
    private readonly JwtTokenProvider _tokenProvider;
    private readonly AccountRepository _accountRepository;
    private readonly RefreshTokenRepository _refreshTokenRepository;
    private readonly PasswordHasher<Entities.Account> _passwordHasher;

    public AuthService(
        JwtTokenProvider tokenProvider,
        AccountRepository accountRepository,
        RefreshTokenRepository refreshTokenRepository,
        PasswordHasher<Entities.Account> passwordHasher
    )
    {
        _tokenProvider = tokenProvider;
        _accountRepository = accountRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _passwordHasher = passwordHasher;
    }

    public Entities.Account registerAccount(AccountDTO account)
    {
        // manual version
        Entities.Account acc = new Entities.Account();
        acc.Id = Guid.NewGuid().ToString();
        acc.Roles = AccountRole.User.ToString();
        acc.Username = account.username;
        acc.PasswordHash = _passwordHasher.HashPassword(acc, account.password);
        var affectedRows = _accountRepository.Add(acc);
        if(affectedRows < 0) throw new ApplicationException("Failed to register account");
        return acc;
    }

    public Dictionary<String, Object> createToken(AccountDTO req)
    {
        // identity version
        
        // manual version
        Entities.Account user = _accountRepository.findByUsername(req.username) ?? throw new ApplicationException("Invalid username");
        var checkPassword = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash ?? "", req.password);
        if(checkPassword == PasswordVerificationResult.Failed) throw new ApplicationException("Invalid password");
        var accessToken = _tokenProvider.generateAcessToken(user);
        var refreshToken = _tokenProvider.generateRefreshToken(user);
        return new Dictionary<String, Object>
        {
            ["access-token"] = accessToken ,
            ["refresh-token"] = refreshToken 
        };
    }

    public Dictionary<String, Object> refresh(RefreshTokenDTO dto)
    {
        var rt = _refreshTokenRepository.findByToken(dto.token);
        Entities.Account user = _accountRepository.FindById(rt.AccountId!);
        var refreshToken = _tokenProvider.generateRefreshToken(user);
        return new Dictionary<String, Object>
        {
            ["refresh-token"] = refreshToken 
        };
    }
}