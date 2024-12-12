using System;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class ResigterDto
{
    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [StringLength(8, MinimumLength =4)]
    public string Password { get; set; } = string.Empty;
}
