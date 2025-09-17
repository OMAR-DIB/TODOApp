
using ToDo.Data.Entities.Base;

namespace ToDo.Data.Entities
{
    public class User : BaseEntity
    {
        public User()
        {
            // Initialize the collection in the constructor to prevent it from being null
            UserRoles = new HashSet<UserRole>();
            Notifications = new HashSet<Notification>();
        }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        // ToDo.Data.Entities.User (add these members)
        public bool IsEmailConfirmed { get; set; } = false;

        public string? EmailVerificationCodeHash { get; set; }   // hashed OTP (SHA256 hex)
        public DateTime? EmailVerificationCodeExpires { get; set; }
        public DateTime? LastVerificationSentAt { get; set; }     // for simple rate limiting
        public ICollection<UserRole> UserRoles { get; set; }
        public ICollection<Notification> Notifications { get; set; } 
    }
}
