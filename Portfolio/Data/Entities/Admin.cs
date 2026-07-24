using System.ComponentModel.DataAnnotations;

namespace Portfolio.Data.Entities
{
    public class Admin
    {

        public int Id { get; set; }
        public string FullName { get; set; }
        public string? ImageUrl { get; set; }
        public string UserName { get; set; }

        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }
        public string? PasswordResetTokenHash { get; set; }
        public DateTime? PasswordResetTokenExpiresAt { get; set; }
    }
}