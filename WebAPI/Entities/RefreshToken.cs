using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WebAPI.Entities;

public partial class RefreshToken
{
    public string Id { get; set; } = null!;
    

    public string AccountId { get; set; } = null!;
    
    public DateTime ExpriyDate { get; set; }

    public string Token { get; set; } = null!;
}
