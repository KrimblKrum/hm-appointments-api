using AppointmentsApi.Data;
using AppointmentsApi.Data.Entities;
using AppointmentsApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentsApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ScheduleController : ControllerBase
    {
        private readonly IAppointmentsDbContext _dbContext;

        public ScheduleController(IAppointmentsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        public IActionResult Create(CreateScheduleRequest model)
        {
            if (!ModelState.IsValid) return BadRequest();

            if (model.EndUtc.ToString("yyyy-MM-dd") != model.StartUtc.ToString("yyyy-MM-dd"))
            {
                return BadRequest("Availability must be during the same day.");
            }

            // TODO: Replace with mapper?
            var entity = new ScheduleEntity
            {
                ScheduleId = Guid.NewGuid(),
                ProviderId = model.ProviderId,
                StartUtc = model.StartUtc,
                EndUtc = model.EndUtc,
            };

            _dbContext.Schedules?.Add(entity);
            _dbContext.SaveChanges();

            return Ok(entity);
        }
    }
}
