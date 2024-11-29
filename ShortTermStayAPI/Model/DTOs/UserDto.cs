using ShortTermStayAPI.Model.Entities;
using System.ComponentModel.DataAnnotations;

public class LoginDTO
{
    [Required]
    public string Email { get; set; }
    
    [Required]
    public string Password { get; set; }
}

public class RegisterDTO
{
    [Required]
    [MaxLength(100)]
    public string Username { get; set; }

    [Required]
    [EmailAddress]
    [MaxLength(100)]
    public string Email { get; set; }

    [Required]
    [MinLength(6)]
    public string Password { get; set; }

    public UserRole Role { get; set; } = UserRole.Guest;
}