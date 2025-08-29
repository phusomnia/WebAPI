using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Features.PolicyAPI;

public class RoleController : ControllerBase
{
    [HttpPost()]
    public IActionResult addPermission()
    {
        return Ok();
    }
}