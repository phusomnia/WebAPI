using Microsoft.AspNetCore.Authorization;

namespace WebAPI.Demo.permission;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class PermissionAttribute : AuthorizeAttribute
{
    public PermissionAttribute(string permission)
    {
        Policy = $"Permission:{permission}";
    }
}