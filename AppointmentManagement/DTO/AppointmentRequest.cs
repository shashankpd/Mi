namespace AppointmentManagement.DTO
{
    public class AppointmentRequest
    {

        public string PATIENTNAME { get; set; }
        public int PATIENTAGE { get; set; }
        public string EMAILID { get; set; }
        public string ISSUE { get; set; }
        public DateTime APPOINTMENTDATE { get; set; }= DateTime.Now;
      
    }
}
