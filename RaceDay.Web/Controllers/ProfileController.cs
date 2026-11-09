using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using RaceDay.Web.Models;

namespace RaceDay.Web.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ProfileController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private HttpClient CreateApiClient()
        {
            var client =
                _httpClientFactory.CreateClient("RaceDayAPI");

            var token =
                HttpContext.Session.GetString("JWTToken");

            if (!string.IsNullOrWhiteSpace(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            return client;
        }

        public async Task<IActionResult> Index()
        {
            var client = CreateApiClient();

            var response =
                await client.GetAsync("api/Profile/me");

            if (!response.IsSuccessStatusCode)
            {
                return View("Error");
            }

            var profile =
                await response.Content
                    .ReadFromJsonAsync<ProfileViewModel>();

            if (profile == null)
            {
                return View("Error");
            }

            return View(profile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var client = CreateApiClient();

            var response =
                await client.PutAsJsonAsync(
                    "api/Profile/me",
                    model);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] =
                    "Your profile has been updated.";

                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(
                "",
                "Your profile could not be updated.");

            return View(model);
        }
    }
}