using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Features.PolicyAPI;

[ApiController]
[Route("/api/v1")]
public class PermissionController : ControllerBase
{
    private readonly PermissionRepository _repo;

    public PermissionController(PermissionRepository repo)
    {
        _repo = repo;
    }

    [HttpPost("permission")]
    public async Task<Object> getPermissionOfAccount(
        [FromBody] PermissionAccountDTO dto
        )
    {
        var result = await _repo.getPermissionByUserId(dto.id);
        return Ok(result.Select(x => x["name"]));
    }
}
public record PermissionAccountDTO(String id);