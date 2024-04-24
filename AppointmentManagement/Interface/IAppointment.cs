using AppointmentManagement.DTO;

namespace AppointmentManagement.Interface
{
    public interface IAppointment
    {
        Task<AppointmentRequest> CreateAppointment(AppointmentRequest appointment, int PatientID, int DoctorID);

        public Task<IEnumerable<AppointmentResponseDto>> GetAllAppointmentsByDoctor(int doctorId);
        public Task<IEnumerable<AppointmentResponseDto>> GetAllAppointmentsByPatient(int patientId);
        public Task<IEnumerable<AppointmentResponseDto>> GetAppointmentsById(int appointmentId);
        public Task<AppointmentResponseDto> UpdateAppointment(AppointmentRequest request, int patientId, int AppointmentId);
        public Task<AppointmentResponseDto> UpdateStatus(int appointmentId, string status);

    }
}
