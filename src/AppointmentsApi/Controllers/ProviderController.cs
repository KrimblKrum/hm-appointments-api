using AppointmentsApi.Data;
using AppointmentsApi.Data.Entities;
using AppointmentsApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentsApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProviderController : ControllerBase
    {
        private readonly IAppointmentsDbContext _dbContext;

        public ProviderController(IAppointmentsDbContext dbContext) 
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        public ActionResult Create(CreateProviderRequest model)
        {
            if (!ModelState.IsValid) return BadRequest();

            // TODO: Replace with mapper?
            var entity = new ProviderEntity
            {
                ProviderId = Guid.NewGuid(),
                Name = model.Name,
            };

            _dbContext.Providers?.Add(entity);
            _dbContext.SaveChanges();

            return Ok(entity);
        }
    }
}
