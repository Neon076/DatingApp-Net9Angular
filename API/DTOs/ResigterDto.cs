using System;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class ResigterDto
{
    [Required]
    [MaxLength(50)]
    public required string Username { get; set; }

    [Required]
    public required string Password { get; set; }
}
