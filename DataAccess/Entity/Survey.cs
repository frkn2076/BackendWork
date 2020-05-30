using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entity {
    public class Survey {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public DateTime ExpireDate { get; set; }
        [Required]
        public decimal Money { get; set; }
        [Required]
        public List<Question> Questions { get; set; }
        public string Condition { get; set; }
    }
}
