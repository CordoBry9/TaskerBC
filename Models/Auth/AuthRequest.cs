using System.ComponentModel.DataAnnotations;

namespace TaskerBC.Models.Auth
{
    public class AuthRequest
    {
        [Required, EmailAddress, MinLength(1)]
        public string? Email { get; set; }

        [Required, MinLength(6)]
        public string? Password { get; set; }
    }

    public class LoginRequest
    {
        [Required, EmailAddress, MinLength(1)]
        public string? Email { get; set; }
        [Required, MinLength(6)]
        public string? Password { get; set; }
    }

    public class RegisterRequest
    {
        [Required, EmailAddress, MinLength(1)]
        public string? Email { get; set; }
        [Required, MinLength(6)]
        public string? Password { get; set; }
    }

    public class RefreshRequest
    {
        public string? RefreshToken { get; set; }
    }
}
