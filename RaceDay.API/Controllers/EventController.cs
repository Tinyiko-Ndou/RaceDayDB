using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;


namespace RaceDay.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class EventController :ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public EventController(RaceDayDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Organiser,Participant")]
        [HttpGet]
        public async Task<IActionResult> GetEvents()
        {
            var events = await _context.Events.ToListAsync();
            return Ok(events);

        }

    }
}
