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
    public class CategoriesController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public CategoriesController(RaceDayDbContext context)
        {
            _context = context;
        }

        // GET: api/Categories
        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _context.Categories
                .AsNoTracking()
                .Select(c => new
                {
                    c.CategoryID,
                    c.CategoryName,
                    c.Description
                })
                .ToListAsync();

            return Ok(categories);
        }

        // GET: api/Categories/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            var category = await _context.Categories
                .AsNoTracking()
                .Where(c => c.CategoryID == id)
                .Select(c => new
                {
                    c.CategoryID,
                    c.CategoryName,
                    c.Description
                })
                .FirstOrDefaultAsync();

            if (category == null)
            {
                return NotFound(new { message = "Category not found." });
            }

            return Ok(category);
        }

        // POST: api/Categories
        [HttpPost]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> CreateCategory(
            CategoryRequest request)
        {
            string name = request.CategoryName.Trim();

            bool exists = await _context.Categories
                .AnyAsync(c => c.CategoryName == name);

            if (exists)
            {
                return Conflict(new
                {
                    message = "A category with this name already exists."
                });
            }

            var category = new Category
            {
                CategoryName = name,
                Description = request.Description
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetCategory),
                new { id = category.CategoryID },
                category);
        }

        // PUT: api/Categories/5
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> UpdateCategory(
            int id,
            CategoryRequest request)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound(new { message = "Category not found." });
            }

            string name = request.CategoryName.Trim();

            bool duplicate = await _context.Categories.AnyAsync(c =>
                c.CategoryID != id && c.CategoryName == name);

            if (duplicate)
            {
                return Conflict(new
                {
                    message = "Another category already uses this name."
                });
            }

            category.CategoryName = name;
            category.Description = request.Description;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Category updated successfully." });
        }

        // DELETE: api/Categories/5
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories
                .Include(c => c.EventCategories)
                .FirstOrDefaultAsync(c => c.CategoryID == id);

            if (category == null)
            {
                return NotFound(new { message = "Category not found." });
            }

            if (category.EventCategories.Any())
            {
                return Conflict(new
                {
                    message = "This category is linked to events. Remove its event associations before deleting it."
                });
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Category deleted successfully." });
        }

        // POST: api/Categories/events/5
        // Associates a category with an event.
        [HttpPost("events/{eventId:int}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> AddCategoryToEvent(
            int eventId,
            EventCategoryRequest request)
        {
            var eventItem = await _context.Events
                .FirstOrDefaultAsync(e => e.EventID == eventId);

            if (eventItem == null)
            {
                return NotFound(new { message = "Event not found." });
            }

            var category = await _context.Categories
                .FindAsync(request.CategoryID);

            if (category == null)
            {
                return NotFound(new { message = "Category not found." });
            }

            var existingAssociation = await _context.EventCategories
                .AnyAsync(ec =>
                    ec.EventID == eventId &&
                    ec.CategoryID == request.CategoryID);

            if (existingAssociation)
            {
                return Conflict(new
                {
                    message = "This category is already linked to the event."
                });
            }

            var association = new EventCategory
            {
                EventID = eventId,
                CategoryID = request.CategoryID,
                EntryFee = request.EntryFee,
                MaxParticipants = request.MaxParticipants
            };

            _context.EventCategories.Add(association);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Category linked to event successfully.",
                association.EventCategoryID,
                association.EventID,
                association.CategoryID,
                association.EntryFee,
                association.MaxParticipants
            });
        }

        // GET: api/Categories/events/5
        [HttpGet("events/{eventId:int}")]
        public async Task<IActionResult> GetEventCategories(int eventId)
        {
            bool eventExists = await _context.Events
                .AnyAsync(e => e.EventID == eventId);

            if (!eventExists)
            {
                return NotFound(new { message = "Event not found." });
            }

            var categories = await _context.EventCategories
                .AsNoTracking()
                .Where(ec => ec.EventID == eventId)
                .Select(ec => new
                {
                    ec.EventCategoryID,
                    ec.EventID,
                    ec.CategoryID,
                    ec.Category.CategoryName,
                    ec.Category.Description,
                    ec.EntryFee,
                    ec.MaxParticipants
                })
                .ToListAsync();

            return Ok(categories);
        }
    }
}