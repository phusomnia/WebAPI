using System;
using System.Collections.Generic;

namespace WebAPI.Entities;

public partial class Permission
{
    public string Id { get; set; } = null!;

    public string? Name { get; set; }

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
