using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using WebAPI.Annotation;

namespace WebAPI.Features.AuthAPI.handler.permission;

[Provider]
public class PermissionPolicyProvider : IAuthorizationPolicyProvider
{
    private const string POLICY_PREFIX = "Permission:";
    private readonly DefaultAuthorizationPolicyProvider _fallbackProvider;
    
    public PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
    {
        _fallbackProvider = new DefaultAuthorizationPolicyProvider(options);
    }
    
    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        Console.WriteLine(policyName);
        if (policyName.StartsWith(POLICY_PREFIX, StringComparison.OrdinalIgnoreCase))
        {
            string permission = policyName.Substring(POLICY_PREFIX.Length);
            var policy = new AuthorizationPolicyBuilder();
            policy.AddRequirements(new PermissionRequirement(permission));
            return Task.FromResult(policy.Build())!;
        }
        return _fallbackProvider.GetPolicyAsync(policyName);
    }
    
    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => _fallbackProvider.GetDefaultPolicyAsync();
    
    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => _fallbackProvider.GetFallbackPolicyAsync();
}