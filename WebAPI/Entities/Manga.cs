using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.Entities;

[Table("Manga")]
public partial class Manga
{
    public string Id { get; set; } = null!;

    public string? Title { get; set; }
}
