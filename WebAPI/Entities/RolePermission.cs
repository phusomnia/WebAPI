using System;
using System.Collections.Generic;

namespace WebAPI.Entities;

public partial class RolePermission
{
    public string RoleId { get; set; } = null!;

    public string PermissionId { get; set; } = null!;
}
