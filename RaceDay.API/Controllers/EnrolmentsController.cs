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
    public class EnrolmentsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public EnrolmentsController(RaceDayDbContext context)
        {
            _context = context;
        }

        private bool TryGetUserId(out int userId)
        {
            return int.TryParse(
                User.FindFirstValue(ClaimTypes.NameIdentifier),
                out userId);
        }

        // POST: api/Enrolments
        [HttpPost]
        [Authorize(Roles = "Participant")]
        public async Task<IActionResult> Enrol(EnrolmentRequest request)
        {
            if (!TryGetUserId(out int userId))
                return Unauthorized();

            var eventCategory = await _context.EventCategories
                .Include(ec => ec.Event)
                .Include(ec => ec.Category)
                .FirstOrDefaultAsync(ec =>
                    ec.EventCategoryID == request.EventCategoryID &&
                    ec.EventID == request.EventID);

            if (eventCategory == null)
            {
                return BadRequest(new
                {
                    message = "The selected category is not linked to this event."
                });
            }

            if (eventCategory.Event.EventDate < DateTime.Today)
            {
                return BadRequest(new
                {
                    message = "You cannot enrol in a past event."
                });
            }

            bool alreadyEnrolled = await _context.Enrolments.AnyAsync(e =>
                e.UserID == userId &&
                e.EventID == request.EventID);

            if (alreadyEnrolled)
            {
                return Conflict(new
                {
                    message = "You are already enrolled in this event."
                });
            }

            int currentCount = await _context.Enrolments.CountAsync(e =>
                e.EventCategoryID == request.EventCategoryID &&
                e.Status != "Cancelled");

            if (currentCount >= eventCategory.MaxParticipants)
            {
                return Conflict(new
                {
                    message = "This event category has reached its participant limit."
                });
            }

            var enrolment = new Enrolment
            {
                UserID = userId,
                EventID = request.EventID,
                EventCategoryID = request.EventCategoryID,
                EnrolmentDate = DateTime.UtcNow,
                Status = "Confirmed"
            };

            _context.Enrolments.Add(enrolment);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Enrolment successful.",
                enrolment.EnrolmentID,
                enrolment.UserID,
                enrolment.EventID,
                enrolment.EventCategoryID,
                enrolment.EnrolmentDate,
                enrolment.Status
            });
        }

        // GET: api/Enrolments/mine
        [HttpGet("mine")]
        [Authorize(Roles = "Participant")]
        public async Task<IActionResult> GetMyEnrolments()
        {
            if (!TryGetUserId(out int userId))
                return Unauthorized();

            var enrolments = await _context.Enrolments
                .AsNoTracking()
                .Where(e => e.UserID == userId)
                .Select(e => new
                {
                    e.EnrolmentID,
                    e.EventID,
                    e.Event.EventName,
                    e.Event.EventDate,
                    e.EventCategoryID,
                    e.EventCategory.Category.CategoryName,
                    e.EnrolmentDate,
                    e.Status
                })
                .ToListAsync();

            return Ok(enrolments);
        }

        // GET: api/Enrolments/event/5
        [HttpGet("event/{eventId:int}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> GetEventEnrolments(int eventId)
        {
            if (!TryGetUserId(out int userId))
                return Unauthorized();

            bool ownsEvent = await _context.Events.AnyAsync(e =>
                e.EventID == eventId &&
                e.OrganizerID == userId);

            if (!ownsEvent)
            {
                return NotFound(new
                {
                    message = "Event not found or you do not own this event."
                });
            }

            var enrolments = await _context.Enrolments
                .AsNoTracking()
                .Where(e => e.EventID == eventId)
                .Select(e => new
                {
                    e.EnrolmentID,
                    e.UserID,
                    ParticipantName = e.User.FirstName + " " + e.User.LastName,
                    e.EventCategoryID,
                    e.EventCategory.Category.CategoryName,
                    e.EnrolmentDate,
                    e.Status
                })
                .ToListAsync();

            return Ok(enrolments);
        }

        // PUT: api/Enrolments/5/cancel
        [HttpPut("{id:int}/cancel")]
        [Authorize(Roles = "Participant")]
        public async Task<IActionResult> CancelEnrolment(int id)
        {
            if (!TryGetUserId(out int userId))
                return Unauthorized();

            var enrolment = await _context.Enrolments
                .FirstOrDefaultAsync(e =>
                    e.EnrolmentID == id && e.UserID == userId);

            if (enrolment == null)
            {
                return NotFound(new
                {
                    message = "Enrolment not found."
                });
            }

            enrolment.Status = "Cancelled";
            await _context.SaveChangesAsync();

            return Ok(new { message = "Enrolment cancelled." });
        }
    }
}