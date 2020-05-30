using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entity {
    public class Jobs {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        public static implicit operator Jobs(string job) {
            return new Jobs() { Name = job };
        }
    }

    
}
