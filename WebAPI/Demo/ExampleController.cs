using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CloudinaryDotNet.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Extensions;
using WebAPI.Annotation;
using WebAPI.Core.filters;
using WebAPI.Core.shared;
using WebAPI.Demo;
using WebAPI.Example.Validate;
using WebAPI.Features.CacheAPI.InMem;

namespace WebAPI.Example;

[ApiController]
[Route("/api/v1/")]
public class ExampleController : ControllerBase
{
    private readonly ILogger<ExampleController> _logger;
    private readonly CacheManager _cache;
    private readonly UserMock _userMock;
    private readonly IConfiguration _config;

    public ExampleController(
        ILogger<ExampleController> logger,
        CacheManager cache,
        UserMock userMock,
        IConfiguration config
    )
    {
        _logger = logger;
        _cache = cache;
        _userMock = userMock;
        _config = config;
    }

    [HttpGet("middleware")]
    [TypeFilter(typeof(CustomActionFilter))]
    public IActionResult Get(
        [FromQuery] string? name,
        [FromHeader] string? header
    )
    {
        foreach (var headerItem in Request.Headers)
        {
            Console.WriteLine("Request Header: {0} = {1}", headerItem.Key, headerItem.Value);
        }

        _logger.LogInformation("Request Header: {0} = {1}", header, header);
        return Ok("Hello " + name);
    }

    [HttpGet("cache")]
    [TypeFilter(typeof(InMemoryCacheFilter))]
    public async Task<IActionResult> Cache()
    {
        // var data = GetDataFromListAsync();
        return Ok("Duma");
    }

    [HttpGet("error")]
    [TypeFilter(typeof(CustomExceptionFilter))]
    public Task<IActionResult> Error()
    {
        _logger.LogError("System error occurred");
        throw new InvalidOperationException("System error occurred");
    }

    [HttpPost("inmem-cache")]
    public IActionResult setCache([FromBody] CacheItem item)
    {
        var options = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(30));
        _cache.Set<object>(item.Key, item.Value, options);
        return Ok(item);
    }

    [HttpGet("inmem-cache/{key}")]
    public IActionResult getCache(string key)
    {
        if (!_cache.tryGetValue<object>(key, out var value))
        {
            return NotFound();
        }

        return Ok(value);
    }

    [HttpGet("pub-sub")]
    public IActionResult pubsub()
    {
        return Ok("pub sub");
    }

    [HttpGet("exception")]
    public IActionResult handleException()
    {
        throw new Exception("Ex");
    }

    [HttpPost("validate")]
    public IActionResult validateDemo(ValidateDTO dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        return Ok("Ok");
    }

    private object GetDataFromListAsync()
    {
        return new List<string>
        {
            "Item 1", "Item 2", "Item 3"
        };
    }
    
    [HttpPost("loginDemo")]
    public IActionResult loginDemo([FromBody] CustomUserDTO login)
    {
        var user = _userMock.Authenticate(login.username, login.password);
        Console.WriteLine(CustomJson.json(user, CustomJsonOptions.WriteIndented));
        var token = genJwtToken(user);
        return Ok(token);
    }

    [HttpGet("authDemo")]
    [Authorize(Policy = "RequireAdmin")]
    public IActionResult authDemo()
    {
        return Ok();
    }

    private string genJwtToken(CustomUser user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:secretKey"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.username),
            new Claim(ClaimTypes.Role, user.role.ToString())
        };

        var token = new JwtSecurityToken(
            _config["Jwt:issuer"],
            _config["Jwt:audience"],
            claims,
            expires: DateTime.Now.AddMinutes(2),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    [HttpGet("secure")]
    [Authorize(Policy = "CustomPolicy")]
    public IActionResult secureEndpoint()
    {
        return Ok("Access granted");
    }
}

class CacheEntryMetadata<T>
{
    public T? Data { get; set; }
    public DateTime ExpirationTime { get; set; }
}

public class CacheItem
{
    public String Key { get; set; }
    public Object Value { get; set; }

    public override string ToString()
    {
        return $"Key: {Key} and value: {Value}";
    }
}

// try to implement role base
public record CustomUserDTO(String username, String password);
public class CustomUser
{
    public String username { get; set; }
    public String password { get; set; }
    public Role role { get; set; }
}

[Component]
public class UserMock
{   
    private readonly List<CustomUser> _users = new List<CustomUser>();

    public UserMock()
    {
        _users.Add(new CustomUser() { username = "admin", password = "123", role = Role.Admin });
        _users.Add(new CustomUser() { username = "user", password = "123", role = Role.User });
    }

    public CustomUser Authenticate(string username, string password)
    {
        return _users.FirstOrDefault(user => user.username == username && user.password == password);
    }
}
