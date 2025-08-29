using Microsoft.AspNetCore.Authorization;
using WebAPI.Annotation;

namespace WebAPI.Features.AuthAPI.handler.permission;

[Provider]
public class PermissionRequirementHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,PermissionRequirement requirement)
    {
        Console.WriteLine("handle requirement");
        if (context.User.HasClaim( "permission", requirement.permission))
        {
            Console.WriteLine(requirement.permission);
            context.Succeed(requirement);
        }
        return Task.CompletedTask;
    }
}

public class PermissionRequirement : IAuthorizationRequirement
{
    public string permission { get; }

    public PermissionRequirement(String permission)
    {
        this.permission = permission;
    }
}


