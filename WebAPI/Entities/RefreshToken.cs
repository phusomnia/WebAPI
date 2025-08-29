using System;
using System.Collections.Generic;

namespace WebAPI.Entities;

public partial class RefreshToken
{
    public string Id { get; set; } = null!;

    public string? Token { get; set; }

    public string? AccountId { get; set; }

    public DateTime? ExpiryDate { get; set; }
}
