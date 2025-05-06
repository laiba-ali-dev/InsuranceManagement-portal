using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnlineInsurance.Models
{
    public class Insurance
    {

        [Key]  // Primary Key
        public int Id { get; set; }

		[Required]
		[StringLength(15, MinimumLength = 3, ErrorMessage = "Name must be within 3 to 15 characters")]
		public string Name { get; set; }

        public string Description { get; set; }

        [Required]  // Ensures Premium is mandatory
        public string Premium { get; set; }

        [Required]  // Ensures Duration is mandatory
        public string Duration { get; set; }

        [Required]
        public string ClaimLimit { get; set; }

        [Required]  // Ensures Duration is mandatory
        public string Type { get; set; }

       
    }
}
