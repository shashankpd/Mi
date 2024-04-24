namespace AppointmentManagement.Entity
{
    public class AppointmentEntity
    {
        public int APPOINTMENTID { get; set; }
        public string PATIENTNAME { get; set; }
        public int PATIENTAGE { get; set; }
        public string EMAILID { get; set; }
        public string ISSUE { get; set; }
        public DateTime APPOINTMENTDATE { get; set; }
        public string DOCTORNAME { get; set; }
        public string Specialization { get; set; }
        public int BOOKEDWITH { get; set; }
        public int BOOKEDBY { get; set; }
        public bool BOOKINGSTATUS { get; set; }

    }
}
