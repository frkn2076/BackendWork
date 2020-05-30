using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entity {
    public class Degree {
        [Key]
        public int Id { get; set; }
        [Required]
        public string DegreeId { get; set; }
        [Required]
        public string DegreeText { get; set; }
    }
}
