using persistences.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace identity_server.EntityFrameworks.Entities
{
    [Table("ms_user")]
    public class UserEntity : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Fullname { get; set; } = string.Empty;
        [Required]
        [MaxLength(100)]
        public string Username { get; set; } = string.Empty;
        [Required]
        [MaxLength(100)]
        public string Password { get; set; } = string.Empty;
        [Required]
        [MaxLength(50)]
        public string Email { get; set; } = string.Empty;
        [Required]
        [MaxLength(15)]
        public string PhoneNo { get; set; } = string.Empty;
        public bool IsAdmin { get; set; } = false;
    }
}
