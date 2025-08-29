using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Features.PolicyAPI;

[ApiController]
[Route("/api/v1")]
public class PermissionRoleController : ControllerBase
{
    private readonly PermissionRepository _repo;

    public PermissionRoleController(PermissionRepository repo)
    {
        _repo = repo;
    }

    [HttpGet("role-permission")]
    public IActionResult get()
    {
        // var query = @"SELECT * FROM Role";
        // var result = _repo.executeSqlRaw(query);
        return Ok(_repo.getAll());
    }
}