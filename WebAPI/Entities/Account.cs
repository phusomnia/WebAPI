using System;
using System.Collections.Generic;

namespace WebAPI.Entities;

public partial class Account
{
    public string Id { get; set; } = null!;

    public string? Password { get; set; }

    public string? Roles { get; set; }

    public string? Username { get; set; }
}
