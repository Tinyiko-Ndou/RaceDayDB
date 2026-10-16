using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.DTOs;

namespace RaceDay.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public ProfileController(RaceDayDbContext context)
        {
            _context = context;
        }

        private bool TryGetUserId(out int userId)
        {
            return int.TryParse(
                User.FindFirstValue(ClaimTypes.NameIdentifier),
                out userId);
        }

        // GET: api/Profile/me
        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            if (!TryGetUserId(out int userId))
                return Unauthorized();

            var user = await _context.Users
                .AsNoTracking()
                .Where(u => u.UserId == userId)
                .Select(u => new
                {
                    u.UserId,
                    u.FirstName,
                    u.LastName,
                    u.Email,
                    u.PhoneNumber,
                    u.Role
                })
                .FirstOrDefaultAsync();

            if (user == null)
                return NotFound(new { message = "User not found." });

            return Ok(user);
        }

        // PUT: api/Profile/me
        [HttpPut("me")]
        public async Task<IActionResult> UpdateMyProfile(
            UpdateProfileRequest request)
        {
            if (!TryGetUserId(out int userId))
                return Unauthorized();

            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                return NotFound(new { message = "User not found." });

            user.FirstName = request.FirstName.Trim();
            user.LastName = request.LastName.Trim();
            user.PhoneNumber = request.PhoneNumber;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Profile updated successfully.",
                user.UserId,
                user.FirstName,
                user.LastName,
                user.Email,
                user.PhoneNumber,
                user.Role
            });
        }
    }
}
