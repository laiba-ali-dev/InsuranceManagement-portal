using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnlineInsurance.Models
{
    public class Motor
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Model { get; set; }

        [Required]
        public string Condition  { get; set; }

        [Required]
        public string VechileNumber { get; set; }

        [Required]
        public int Added_by { get; set; }

    }
}
