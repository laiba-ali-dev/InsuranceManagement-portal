namespace OnlineInsurance.Models
{
    public class InsuranceRequestViewModel
    {
        public int Id { get; set; }
        public int InsuranceId { get; set; }

        public int DetailId { get; set; }

        //public string InsuranceType { get; set; } // New property for Insurance Name
        public string InsuranceType { get; set; }

        public string Title { get; set; } // New property for Insurance Name

        public int Added_by { get; set; }

        public string Requested_by { get; set; }


        public string Remarks { get; set; }
        public string Status { get; set; }


    }
}
