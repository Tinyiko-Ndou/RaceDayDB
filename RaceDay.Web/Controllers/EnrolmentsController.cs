using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using RaceDay.Web.Models;

namespace RaceDay.Web.Controllers
{
    [Authorize(Roles = "Participant")]
    public class EnrolmentsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public EnrolmentsController(IHttpClientFactory httpClientFactory)
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            CreateEnrolmentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var client = CreateApiClient();

            var response =
                await client.PostAsJsonAsync(
                    "api/Enrolments",
                    model);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] =
                    "You have successfully enrolled.";

                return RedirectToAction(nameof(Mine));
            }

            TempData["Error"] =
                "Your enrolment could not be completed.";

            return RedirectToAction(
                "Index",
                "BrowseEvents");
        }

        public async Task<IActionResult> Mine()
        {
            var client = CreateApiClient();

            var response =
                await client.GetAsync("api/Enrolments/mine");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] =
                    "Unable to load your enrolments.";

                return View(new List<EnrolmentViewModel>());
            }

            var enrolments =
                await response.Content
                    .ReadFromJsonAsync<List<EnrolmentViewModel>>();

            return View(
                enrolments ?? new List<EnrolmentViewModel>());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var client = CreateApiClient();

            var response =
                await client.PutAsync(
                    $"api/Enrolments/{id}/cancel",
                    null);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] =
                    "Your enrolment has been cancelled.";
            }
            else
            {
                TempData["Error"] =
                    "Unable to cancel this enrolment.";
            }

            return RedirectToAction(nameof(Mine));
        }
    }
}