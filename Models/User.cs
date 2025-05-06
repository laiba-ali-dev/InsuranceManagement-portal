using System.Xml.Linq;
using System;
using System.ComponentModel.DataAnnotations;

namespace OnlineInsurance.Models
{
    public class User
    {

        [Key]
        public int Id { get; set; }

		[Required]
		[StringLength(15, MinimumLength = 3, ErrorMessage = "Name must be within 3 to 15 characters")]
		public string Name { get; set; }

       
      
		[Required]
		[RegularExpression("^([\\w\\.\\-]+)@([\\w\\-]+)((\\.(\\w){2,3})+)$", ErrorMessage = "Email Invalid")]

		public string Email { get; set; }

        [Required]
        [StringLength(15)]
        public string Phone { get; set; }


		[Required]
		[DataType(DataType.Password)]
		[RegularExpression("(?=^.{8,}$)((?=.*\\d)|(?=.*\\W+))(?![.\\n])(?=.*[A-Z])(?=.*[a-z]).*$", ErrorMessage = "Uppercase,Lowercase,Numbers,Symbols Min 8 Chars")]

		public string Password { get; set; }

        [Required]
        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }


    }


}
