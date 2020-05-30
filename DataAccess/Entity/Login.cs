using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entity {
    public class Login {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string MailKey { get; set; }
        public bool IsActive { get; set; }
    }
}
