using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaceDay.API.Services;

namespace RaceDay.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UploadsController : ControllerBase
    {
        private readonly BlobStorageService _storage;

        public UploadsController(BlobStorageService storage)
        {
            _storage = storage;
        }

        [HttpPost("event-banner")]
        [Authorize(Roles = "Organiser")]
        [RequestSizeLimit(5 * 1024 * 1024)]
        public async Task<IActionResult> UploadEventBanner(
            IFormFile file)
        {
            try
            {
                var blobName =
                    await _storage.UploadImageAsync(
                        file,
                        "event-banners");

                return Ok(new
                {
                    message = "Event banner uploaded.",
                    blobName
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("profile-picture")]
        [Authorize(Roles = "Participant")]
        [RequestSizeLimit(5 * 1024 * 1024)]
        public async Task<IActionResult> UploadProfilePicture(
            IFormFile file)
        {
            try
            {
                var blobName =
                    await _storage.UploadImageAsync(
                        file,
                        "profile-pictures");

                return Ok(new
                {
                    message = "Profile picture uploaded.",
                    blobName
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}