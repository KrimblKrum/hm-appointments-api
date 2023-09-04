using AppointmentsApi.Data;
using AppointmentsApi.Data.Entities;
using AppointmentsApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentsApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClientController : ControllerBase
    {
        private readonly IAppointmentsDbContext _dbContext;

        public ClientController(IAppointmentsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        public ActionResult Create(CreateClientRequest model)
        {
            if (!ModelState.IsValid) return BadRequest();

            // TODO: Replace with mapper?
            var entity = new ClientEntity
            {
                ClientId = Guid.NewGuid(),
                Name = model.Name,
            };

            _dbContext.Clients?.Add(entity);
            _dbContext.SaveChanges();

            return Ok(entity);
        }
    }
}
