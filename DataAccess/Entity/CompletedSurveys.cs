using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entity {
    public class CompletedSurveys {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public int SurveyId { get; set; }
    }
}
