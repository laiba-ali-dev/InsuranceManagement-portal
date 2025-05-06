using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnlineInsurance.Models
{
    public class Life
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string BankStatement { get; set; }

		[Required]
		[RegularExpression("^[0-9]{5}-[0-9]{7}-[0-9]$", ErrorMessage = "Cnic Invalid")]
		public string CnicNo { get; set; }

        [Required]
        public string HealthInsurance { get; set; }

        [Required]
        public int Added_by { get; set; }



    }
}
