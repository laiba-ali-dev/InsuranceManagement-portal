using System.ComponentModel.DataAnnotations;

namespace OnlineInsurance.Models
{
    public class InsuranceRequest
    {
        [Key]
        public int Id { get; set; }

        public int InsuranceId { get; set; }

        public int DetailId { get; set; }

        public int Added_by { get; set; }

        public string Remarks { get; set; }

        public string Action { get; set; }

      


    }
}
