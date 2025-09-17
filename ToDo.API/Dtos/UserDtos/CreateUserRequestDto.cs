using System.ComponentModel.DataAnnotations;

namespace ToDo.API.Dtos.UserDtos
{
    public class CreateUserRequestDto 
    {
        [Required, MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [Required, EmailAddress] // Added Email property since User entity has Email
        public string Email { get; set; } = string.Empty;
    }
}
