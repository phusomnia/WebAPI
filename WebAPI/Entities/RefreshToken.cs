using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.Entities;

[Table("RefreshToken")]
public partial class RefreshToken
{
    public string Id { get; set; } = null!;

    public string? Token { get; set; }

    public string? AccountId { get; set; }

    public DateTime? ExpiryDate { get; set; }
}
