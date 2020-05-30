using DataAccess.enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entity {
    public class UserInfo {
        [Key]
        public ulong Tckn { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
        public DateTime BirthDate { get; set; }

        /// <summary>
        /// //////////////////////////////////////////////////////////////////////////
        /// </summary>
        
        public string City { get; set; }
        public string Town { get; set; }
        public Jobs Job { get; set; }
        [EnumDataType(typeof(Sex))]
        public Sex Sex { get; set; }
    }
}
