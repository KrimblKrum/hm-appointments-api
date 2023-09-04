using AppointmentsApi.Data;
using AppointmentsApi.Models;
using AppointmentsApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentsApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [HttpGet]
        [Route("Availability/{providerId}")]
        public IActionResult GetAvailability(GetAvailabilityRequest model)
        {
            if (!ModelState.IsValid) return BadRequest();

            var requestDate = model.RequestDate.GetValueOrDefault();

            return Ok(_appointmentService.GetAvailability(model.ProviderId, requestDate.Date));
        }

        [HttpPost]
        public IActionResult Create(CreateAppointmentRequest model)
        {
            if (!ModelState.IsValid) return BadRequest();

            if (model.EndUtc.Date != model.StartUtc.Date)
            {
                return BadRequest("Availability must be during the same day.");
            }

            if (model.StartUtc.Date == DateTime.UtcNow.Date)
            {
                return BadRequest("Reservations must be made at least 24 hours in advance");
            }

            return Ok(_appointmentService.CreateAppointment(model));
        }

        [HttpPost]
        [Route("Availability/Confirm/{appointmentId}")]
        public IActionResult Confirm(Guid appointmentId)
        {
            if (appointmentId == Guid.Empty) return BadRequest($"{nameof(appointmentId)} must be a valid Guid.");

            if (_appointmentService.ConfirmAppointment(appointmentId))
            {
                return Ok(new { Message = $"Successfully confirmed appointment with {nameof(appointmentId)} of '{appointmentId}'." });
            }

            return NotFound(new { Message = $"Failed to confirm appointment with {nameof(appointmentId)} of '{appointmentId}'." });
        }
    }
}
