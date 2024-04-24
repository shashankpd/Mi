using AppointmentManagement.DTO;
using AppointmentManagement.Entity;
using AppointmentManagement.Interface;
using Dapper;
using Repository.Context;
using System.Net;

namespace AppointmentManagement.Service
{
    public class AppointmentService : IAppointment
    {

        private readonly DapperContext _context;
        private readonly IHttpClientFactory httpClientFactory;

        public AppointmentService(DapperContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<AppointmentRequest> CreateAppointment(AppointmentRequest appointment, int PatientID, int DoctorID)
        {
            try
            {
                string insertQuery = @"INSERT INTO APPOINTMENT (PATIENTNAME, PATIENTAGE, EMAILID, ISSUE, DOCTORNAME, Specialization,
                           APPOINTMENTDATE, BOOKINGSTATUS, BOOKEDWITH, BOOKEDBY)
                           VALUES (@PatientName, @PatientAge, @EmailId, @Issue, @DoctorName, @Specialization,
                           @AppointmentDate, @BookingStatus, @BookedWith, @BookedBy);
                           SELECT SCOPE_IDENTITY();";

                var doctor = await getDoctorById(DoctorID);
                if (doctor == null)
                {
                    throw new Exception("Doctor not found.");
                }

                AppointmentEntity appointmentEntity = MapToEntity(appointment, doctor, PatientID);

                using (var connection = _context.CreateConnection())
                {
                    var appointmentId = await connection.ExecuteScalarAsync<int>(insertQuery, appointmentEntity);
                    // Query the newly added appointment from the database
                    string selectQuery = @"SELECT APPOINTMENTID, PATIENTNAME, PATIENTAGE, EMAILID, ISSUE,APPOINTMENTDATE, DOCTORNAME, Specialization, 
                                  BOOKEDWITH,BOOKEDBY, BOOKINGSTATUS FROM APPOINTMENT WHERE APPOINTMENTID = @AppointmentId";
                    var addedAppointment = await connection.QueryFirstOrDefaultAsync<AppointmentRequest>(selectQuery, new { AppointmentId = appointmentId });
                    return addedAppointment;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        private AppointmentEntity MapToEntity(AppointmentRequest request, DoctorEntity userObject, int PatientId)
        {
            return new AppointmentEntity
            {
                PATIENTNAME = request.PATIENTNAME,
                PATIENTAGE = request.PATIENTAGE,
                EMAILID = request.EMAILID,
                ISSUE = request.ISSUE,
                DOCTORNAME = userObject.DoctorName,
                Specialization = userObject.Specialization,
                APPOINTMENTDATE = DateTime.Now,
                BOOKINGSTATUS = false,
                BOOKEDWITH = userObject.DoctorId,
                BOOKEDBY = PatientId
            };
        }
        public async Task<DoctorEntity> getDoctorById(int doctorId)
        {
            var httpclient = httpClientFactory.CreateClient("GetByDoctorId");
            var response = await httpclient.GetAsync($"Getdoctorbyid{doctorId}");
            Console.WriteLine(response+"this is the data");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<DoctorEntity>();
               
                if (result != null)
                {
                    Console.WriteLine("Doctor found:");
                    Console.WriteLine($"Doctor ID: {result.DoctorId}, Name: {result.DoctorName}");
                    return result;
                }
                else
                {
                    Console.WriteLine("Doctor not found.");
                    return null; // or throw new DoctorNotFoundException("Doctor not found.");
                }
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                Console.WriteLine($"Doctor with ID {doctorId} not found.");
                return null; // or throw new DoctorNotFoundException($"Doctor with ID {doctorId} not found.");
            }
            else
            {
                // Log the error status code and message
                Console.WriteLine($"Error fetching doctor data: {response.StatusCode}");
                // Handle other error scenarios (e.g., server error)
                return null; // or throw new HttpRequestException($"Error fetching doctor data: {response.StatusCode}");
            }
        }

        public async Task<IEnumerable<AppointmentResponseDto>> GetAllAppointmentsByPatient(int patientId)
        {
            try
            {
                string selectQuery = @"SELECT * FROM APPOINTMENT WHERE BOOKEDBY = @PatientId;";
                using (var connection = _context.CreateConnection())
                {
                    return await connection.QueryAsync<AppointmentResponseDto>(selectQuery, new { PatientId = patientId });
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving appointments by patient.", ex);
            }
        }

        public async Task<IEnumerable<AppointmentResponseDto>> GetAllAppointmentsByDoctor(int doctorId)
        {
            try
            {
                string selectQuery = @"SELECT * FROM APPOINTMENT WHERE BOOKEDWITH = @DoctorId;";
                using (var connection = _context.CreateConnection())
                {
                    return await connection.QueryAsync<AppointmentResponseDto>(selectQuery, new { DoctorId = doctorId });
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving appointments by doctor.", ex);
            }
        }

        public async Task<IEnumerable<AppointmentResponseDto>> GetAppointmentsById(int appointmentId)
        {
            try
            {
                string selectQuery = @"SELECT * FROM APPOINTMENT WHERE APPOINTMENTID = @AppointmentId;";
                using (var connection = _context.CreateConnection())
                {
                    return await connection.QueryAsync<AppointmentResponseDto>(selectQuery, new { AppointmentId = appointmentId });
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving appointments by doctor.", ex);
            }
        }

        public async Task<AppointmentResponseDto> UpdateAppointment(AppointmentRequest request, int patientId, int AppointmentId)
        {
            AppointmentEntity existingAppointment = GetAppointmentsbyId(AppointmentId);
            if (existingAppointment == null)
            {
                throw new Exception("Appointment not found");
            }
            existingAppointment.PATIENTNAME = request.PATIENTNAME;
            existingAppointment.PATIENTAGE = request.PATIENTAGE;
            existingAppointment.ISSUE = request.ISSUE;
            existingAppointment.APPOINTMENTDATE = DateTime.Now;

            string sql = @" UPDATE APPOINTMENT SET PATIENTNAME = @PatientName, 
        PATIENTAGE = @PATIENTAGE, ISSUE = @Issue, BOOKEDBY = @BookedBy, APPOINTMENTDATE = @AppointmentDate WHERE APPOINTMENTID = @AppointmentId";

            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(sql, existingAppointment);

                // Assuming AppointmentResponseDto needs to be retrieved after the update
                return await connection.QueryFirstOrDefaultAsync<AppointmentResponseDto>("SELECT * FROM APPOINTMENT WHERE APPOINTMENTID = @AppointmentId", new { AppointmentId = AppointmentId });
            }
        }


        public async Task<AppointmentResponseDto> UpdateStatus(int appointmentId, string status)
        {
            string query = "UPDATE APPOINTMENT SET BOOKINGSTATUS = @Status WHERE APPOINTMENTID = @AppointmentId";

            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, new { Status = status, AppointmentId = appointmentId });

                // Assuming AppointmentResponseDto needs to be retrieved after the update
                return await connection.QueryFirstOrDefaultAsync<AppointmentResponseDto>("SELECT * FROM APPOINTMENT WHERE APPOINTMENTID = @AppointmentId", new { AppointmentId = appointmentId });
            }
        }

        private AppointmentEntity GetAppointmentsbyId(int appointmentId)
        {
            try
            {
                string selectQuery = @"SELECT * FROM APPOINTMENT WHERE APPOINTMENTID = @AppointmentId;";
                using (var connection = _context.CreateConnection())
                {
                    return connection.QueryFirstOrDefault<AppointmentEntity>(selectQuery, new { AppointmentId = appointmentId });
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving appointment by ID.", ex);
            }
        }



    }

    }
