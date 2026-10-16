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
    public class ResultsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public ResultsController(RaceDayDbContext context)
        {
            _context = context;
        }

        private bool TryGetUserId(out int userId)
        {
            return int.TryParse(
                User.FindFirstValue(ClaimTypes.NameIdentifier),
                out userId);
        }

        // POST: api/Results/enrolment/5
        [HttpPost("enrolment/{enrolmentId:int}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> RecordResult(
            int enrolmentId,
            ResultRequest request)
        {
            if (!TryGetUserId(out int userId))
                return Unauthorized();

            if (request.FinishTime <= TimeSpan.Zero)
            {
                return BadRequest(new
                {
                    message = "Finish time must be greater than zero."
                });
            }

            var enrolment = await _context.Enrolments
                .Include(e => e.Event)
                .Include(e => e.Result)
                .FirstOrDefaultAsync(e => e.EnrolmentID == enrolmentId);

            if (enrolment == null)
            {
                return NotFound(new { message = "Enrolment not found." });
            }

            if (enrolment.Event.OrganizerID != userId)
            {
                return Forbid();
            }

            if (enrolment.Result != null)
            {
                return Conflict(new
                {
                    message = "A result has already been recorded for this enrolment."
                });
            }

            bool positionTaken = await _context.Results
                .AnyAsync(r =>
                    r.Position == request.Position &&
                    r.Enrolment.EventID == enrolment.EventID);

            if (positionTaken)
            {
                return Conflict(new
                {
                    message = "That finishing position is already assigned in this event."
                });
            }

            var result = new Result
            {
                EnrolmentID = enrolmentId,
                FinishTime = request.FinishTime,
                Position = request.Position,
               // Pace = request.Pace,
                ResultStatus = request.ResultStatus
            };

            _context.Results.Add(result);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Result recorded successfully.",
                result.ResultID,
                result.EnrolmentID,
                result.FinishTime,
                result.Position,
                result.Pace,
                result.ResultStatus
            });
        }

        // GET: api/Results/mine
        [HttpGet("mine")]
        [Authorize(Roles = "Participant")]
        public async Task<IActionResult> GetMyResults()
        {
            if (!TryGetUserId(out int userId))
                return Unauthorized();

            var results = await _context.Results
                .AsNoTracking()
                .Where(r => r.Enrolment.UserID == userId)
                .Select(r => new
                {
                    r.ResultID,
                    r.EnrolmentID,
                    r.Enrolment.EventID,
                    r.Enrolment.Event.EventName,
                    r.FinishTime,
                    r.Position,
                    r.Pace,
                    r.ResultStatus
                })
                .ToListAsync();

            return Ok(results);
        }

        // GET: api/Results/event/5
        [HttpGet("event/{eventId:int}")]
        public async Task<IActionResult> GetEventResults(int eventId)
        {
            bool eventExists = await _context.Events
                .AnyAsync(e => e.EventID == eventId);

            if (!eventExists)
                return NotFound(new { message = "Event not found." });

            var results = await _context.Results
                .AsNoTracking()
                .Where(r => r.Enrolment.EventID == eventId)
                .OrderBy(r => r.Position)
                .Select(r => new
                {
                    r.ResultID,
                    r.EnrolmentID,
                    ParticipantName =
                        r.Enrolment.User.FirstName + " " +
                        r.Enrolment.User.LastName,
                    r.FinishTime,
                    r.Position,
                    r.Pace,
                    r.ResultStatus
                })
                .ToListAsync();

            return Ok(results);
        }
    }
}
