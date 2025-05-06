using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnlineInsurance.Models
{
	public class Admin
	{

		[Key]
		public int Id { get; set; }

		[Required]
		[StringLength(15, MinimumLength = 3, ErrorMessage = "Name must be within 3 to 15 characters")]
		public string Name { get; set; }

		[Required]
		[RegularExpression("^([\\w\\.\\-]+)@([\\w\\-]+)((\\.(\\w){2,3})+)$", ErrorMessage = "Email Invalid")]
		public string Email { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [StringLength(12, ErrorMessage = "Phone number cannot exceed 12 digits")]
        public string Phone { get; set; }



        [Required]
		[DataType(DataType.Password)]
		[RegularExpression("(?=^.{8,}$)((?=.*\\d)|(?=.*\\W+))(?![.\\n])(?=.*[A-Z])(?=.*[a-z]).*$", ErrorMessage = "Uppercase,Lowercase,Numbers,Symbols Min 8 Chars")]

		public string Password { get; set; }
	}
}
