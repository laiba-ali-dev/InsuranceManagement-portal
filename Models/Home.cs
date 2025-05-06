using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnlineInsurance.Models
{
    public class Home
    {

        [Key]  // Primary Key
        public int Id { get; set; }

		[Required]
		[StringLength(15, MinimumLength = 3, ErrorMessage = "OwnerName must be within 3 to 15 characters")]
		public string OwnerName { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        public string Stories { get; set; }


        [Required]
        public int  Added_by { get; set; }

        [Required]
        public string Papers { get; set; }


    }
}

