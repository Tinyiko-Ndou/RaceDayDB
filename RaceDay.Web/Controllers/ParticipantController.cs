using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using RaceDay.Web.Models;

namespace RaceDay.Web.Controllers
{
    [Authorize(Roles = "Participant")]
    public class ParticipantController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ParticipantController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private HttpClient CreateApiClient()
        {
            var client = _httpClientFactory.CreateClient("RaceDayAPI");

            var token = HttpContext.Session.GetString("JWTToken");

            if (!string.IsNullOrWhiteSpace(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            return client;
        }

        public async Task<IActionResult> Dashboard()
        {
            var client = CreateApiClient();

            var enrolmentResponse =
                await client.GetAsync("api/Enrolments/mine");

            var enrolments = enrolmentResponse.IsSuccessStatusCode
                ? await enrolmentResponse.Content
                    .ReadFromJsonAsync<List<EnrolmentViewModel>>()
                : new List<EnrolmentViewModel>();


            enrolments ??= new List<EnrolmentViewModel>();

            var model = new ParticipantDashboardViewModel
            {
                TotalEnrolments = enrolments.Count,
                UpcomingEvents = enrolments.Count(e =>
                    e.Status.Equals("Confirmed",
                    StringComparison.OrdinalIgnoreCase)),
                CompletedEvents = enrolments.Count(e =>
                    e.Status.Equals("Completed",
                    StringComparison.OrdinalIgnoreCase)),
                TotalResults = 0
            };

            return View(model);
        }
    }
}