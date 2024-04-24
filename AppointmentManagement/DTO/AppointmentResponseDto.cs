namespace AppointmentManagement.DTO
{
    public class AppointmentResponseDto
    {
        public int APPOINTMENTID { get; set; }
        public string PATIENTNAME { get; set; }
        public int PATIENTAGE { get; set; }
        public string ISSUE { get; set; }
        public string DOCTORNAME { get; set; }
        public string Specialization { get; set; }
        public DateTime APPOINTMENTDATE { get; set; }
        public bool BOOKINGSTATUS { get; set; }
    }
}
