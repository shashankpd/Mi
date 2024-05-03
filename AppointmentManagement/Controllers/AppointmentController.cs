using AppointmentManagement.DTO;
using AppointmentManagement.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AppointmentManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentController : Controller
    {
        private readonly IAppointment _appointmentService;
        
        private readonly HttpClient _httpClient;

        public AppointmentController(IAppointment appointmentService, IHttpClientFactory httpClientFactory)
        {
            _appointmentService = appointmentService;
            _httpClient = httpClientFactory.CreateClient("GetByDoctorId");
        }


        [Authorize(Roles = "Patient")]
        [HttpPost("{DoctorID}")]
        public async Task<IActionResult> CreateAppointment([FromBody] AppointmentRequest appointment, int DoctorID)

        {
            try
            {
                var patientIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
               
                int patientId = Convert.ToInt32(patientIdClaim);
               
                var addedAppointment = await _appointmentService.CreateAppointment(appointment, patientId, DoctorID);
                return Ok(new { Success = true, Message = "Appointment added successfully", Data = addedAppointment });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Patient")]
        [HttpGet("GetByPatient")]
        public async Task<IActionResult> GetAllAppointmentsByPatient()
        {
            try
            {
                var patientId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var appointments = await _appointmentService.GetAllAppointmentsByPatient(patientId);
                return Ok(new{ Success = true, Message = "Retrieved patient successfully", Data = appointments });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Doctor")]
        [HttpGet("GetByDoctor")]
        public async Task<IActionResult> GetAllAppointmentsByDoctor()
        {
            try
            {
                var doctorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var appointments = await _appointmentService.GetAllAppointmentsByDoctor(doctorId);
                return Ok(new { Success = true, Message = "success", Data = appointments });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Patient,Doctor,Admin")]
        [HttpGet("GetAppointmentById")]
        public async Task<IActionResult> GetAppointmentsById()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var appointments = await _appointmentService.GetAppointmentsById(userId);
                return Ok(new { Success = true, Message = "success", Data = appointments });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Patient")]
        [HttpPut("UpdateAppointment")]
        public async Task<AppointmentResponseDto> UpdateAppointment(AppointmentRequest request, int AppointmentId)//-->by patient
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            return await _appointmentService.UpdateAppointment(request, userId, AppointmentId);
        }

        [Authorize(Roles = "Doctor")]
        [HttpPost("UpdateStatus")]
        public async Task<AppointmentResponseDto> UpdateStatus(int Appointmentid, string status)//-->by Doctor
        {
            return await _appointmentService.UpdateStatus(Appointmentid, status);
        }

    }
}
