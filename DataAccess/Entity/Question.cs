using DataAccess.enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entity {
    public class Question {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Text { get; set; }
        public List<ChoosableOption> ChoosableOptions { get; set; }
        public List<Degree> Degree { get; set; }
        [Required]
        [EnumDataType(typeof(QuestionType))]
        public QuestionType Type { get; set; }
        
    }
}
