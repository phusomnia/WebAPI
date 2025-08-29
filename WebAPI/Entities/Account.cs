using System;
using System.Collections.Generic;

namespace WebAPI.Entities;

public partial class Account
{
    public string Id { get; set; } = null!;

    public string? Username { get; set; }

    public string? PasswordHash { get; set; }

    public string RoleId { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;
}
