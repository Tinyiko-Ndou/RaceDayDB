using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using RaceDay.API.Models;

namespace RaceDay.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EventController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public EventController(RaceDayDbContext context)
        {
            _context = context;
        }

        private bool TryGetUserId(out int userId)
        {
            return int.TryParse(
                User.FindFirstValue(ClaimTypes.NameIdentifier),
                out userId);
        }

        // GET: api/Event
        [HttpGet]
        public async Task<IActionResult> GetEvents()
        {
            var events = await _context.Events
                .AsNoTracking()
                .Select(e => new
                {
                    e.EventID,
                    e.EventName,
                    e.Description,
                    e.EventType,
                    e.EventDate,
                    e.StartTime,
                    e.Location,
                    e.City,
                    e.Province,
                    e.Status,
                    e.OrganizerID
                })
                .ToListAsync();

            return Ok(events);
        }

        // GET: api/Event/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetEvent(int id)
        {
            var eventItem = await _context.Events
                .AsNoTracking()
                .Where(e => e.EventID == id)
                .Select(e => new
                {
                    e.EventID,
                    e.EventName,
                    e.Description,
                    e.EventType,
                    e.EventDate,
                    e.StartTime,
                    e.Location,
                    e.City,
                    e.Province,
                    e.Status,
                    e.OrganizerID
                })
                .FirstOrDefaultAsync();

            if (eventItem == null)
            {
                return NotFound(new { message = "Event not found." });
            }

            return Ok(eventItem);
        }

        // POST: api/Event
        [HttpPost]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> CreateEvent(EventRequest request)
        {
            if (!TryGetUserId(out int userId))
            {
                return Unauthorized();
            }

            if (!IsValidEventType(request.EventType))
            {
                return BadRequest(new
                {
                    message = "EventType must be Run, Walk or Cycle."
                });
            }

            if (request.EventDate == default)
            {
                return BadRequest(new
                {
                    message = "A valid event date is required."
                });
            }

            var eventItem = new Event
            {
                EventName = request.EventName.Trim(),
                Description = request.Description,
                EventType = request.EventType.Trim(),
                EventDate = request.EventDate,
                StartTime = request.StartTime,
                Location = request.Location.Trim(),
                City = request.City,
                Province = request.Province,
                Status = request.Status,
                OrganizerID = userId
            };

            _context.Events.Add(eventItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetEvent),
                new { id = eventItem.EventID },
                new
                {
                    message = "Event created successfully.",
                    eventItem.EventID,
                    eventItem.EventName,
                    eventItem.OrganizerID
                });
        }

        // PUT: api/Event/5
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> UpdateEvent(
            int id,
            EventRequest request)
        {
            if (!TryGetUserId(out int userId))
            {
                return Unauthorized();
            }

            var eventItem = await _context.Events
                .FirstOrDefaultAsync(e => e.EventID == id);

            if (eventItem == null)
            {
                return NotFound(new { message = "Event not found." });
            }

            if (eventItem.OrganizerID != userId)
            {
                return Forbid();
            }

            if (!IsValidEventType(request.EventType))
            {
                return BadRequest(new
                {
                    message = "EventType must be Run, Walk or Cycle."
                });
            }

            eventItem.EventName = request.EventName.Trim();
            eventItem.Description = request.Description;
            eventItem.EventType = request.EventType.Trim();
            eventItem.EventDate = request.EventDate;
            eventItem.StartTime = request.StartTime;
            eventItem.Location = request.Location.Trim();
            eventItem.City = request.City;
            eventItem.Province = request.Province;
            eventItem.Status = request.Status;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Event updated successfully." });
        }

        // DELETE: api/Event/5
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            if (!TryGetUserId(out int userId))
            {
                return Unauthorized();
            }

            var eventItem = await _context.Events
                .FirstOrDefaultAsync(e => e.EventID == id);

            if (eventItem == null)
            {
                return NotFound(new { message = "Event not found." });
            }

            if (eventItem.OrganizerID != userId)
            {
                return Forbid();
            }

            _context.Events.Remove(eventItem);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Event deleted successfully." });
        }

        private static bool IsValidEventType(string? eventType)
        {
            return eventType != null &&
                (eventType.Equals("Run",
                     StringComparison.OrdinalIgnoreCase) ||
                 eventType.Equals("Walk",
                     StringComparison.OrdinalIgnoreCase) ||
                 eventType.Equals("Cycle",
                     StringComparison.OrdinalIgnoreCase));
        }
    }
}