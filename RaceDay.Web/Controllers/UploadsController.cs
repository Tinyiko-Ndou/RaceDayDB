using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace RaceDay.Web.Controllers
{
    [Authorize]
    public class UploadsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public UploadsController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public IActionResult EventBanner()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> EventBanner(
            IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("", "Select an image.");
                return View();
            }

            var client =
                _httpClientFactory.CreateClient("RaceDayAPI");

            var token =
                HttpContext.Session.GetString("JWTToken");

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            using var content = new MultipartFormDataContent();

            await using var stream = file.OpenReadStream();

            using var fileContent = new StreamContent(stream);

            fileContent.Headers.ContentType =
                new MediaTypeHeaderValue(file.ContentType);

            content.Add(fileContent, "file", file.FileName);

            var response =
                await client.PostAsync(
                    "api/Uploads/event-banner",
                    content);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Banner uploaded.";
                return RedirectToAction("Index", "Events");
            }

            ModelState.AddModelError(
                "",
                "The API could not upload the image.");

            return View();
        }
    }
}