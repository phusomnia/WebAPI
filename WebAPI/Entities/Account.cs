using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.Entities;

[Table("Account")]
public partial class Account
{
    public string Id { get; set; } = null!;

    public string? Username { get; set; }

    public string? Roles { get; set; }

    public string? PasswordHash { get; set; }
}
