using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnlineInsurance.Models
{
    public class Health
    {
        [Key]
        public int Id { get; set; }

		[Required]
		[StringLength(15, MinimumLength = 3, ErrorMessage = "Name must be within 3 to 15 characters")]
		public string Name { get; set; }

        [Required]
		[RegularExpression("^[0-9]{5}-[0-9]{7}-[0-9]$", ErrorMessage = "Cnic Invalid")]

		public string CnicNo { get; set; }

        [Required]
        public string HealthDocument { get; set; }

        [Required]
        public string Image { get; set; }

        [Required]
        public int Added_by { get; set; }
    }
}
