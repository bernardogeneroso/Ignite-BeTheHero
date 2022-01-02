using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class LoginDto
{
  [Required]
  [EmailAddress]
  public string Email { get; set; }

  [Required]
  [RegularExpression("(?=.*\\d)(?=.*[a-z])(?=.*[A-Z]).{4,8}$", ErrorMessage = "Password must be complex")]
  public string Password { get; set; }
}
