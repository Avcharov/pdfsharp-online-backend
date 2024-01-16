using System.ComponentModel.DataAnnotations.Schema;

namespace pdfsharp_online_backend.Domain
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(200)]
        public string UPN { get; set; }
        public string DisplayName { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
        public int RoleId { get; set; }
        [ForeignKey(nameof(RoleId))]
        public virtual Role? Role { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public string? ResetPasswordToken { get; set; }
        public DateTime ResetPasswordExpiry { get; set; }
    }
}
