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

        public ParticipantController(
            IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Dashboard()
        {
            var dashboard = new ParticipantDashboardViewModel();

            var token = HttpContext.Session.GetString("JWTToken");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            var client = _httpClientFactory.CreateClient("RaceDayAPI");

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            // Retrieve all events
            var eventResponse = await client.GetAsync("api/Event");

            if (eventResponse.IsSuccessStatusCode)
            {
                var events = await eventResponse.Content
                    .ReadFromJsonAsync<List<EventViewModel>>();

                if (events != null)
                {
                    var today = DateTime.Today;

                    dashboard.UpcomingEvents = events.Count(e =>
                        e.EventDate.Date >= today);

                    dashboard.CompletedEvents = events.Count(e =>
                        e.EventDate.Date < today);
                }
            }

            // Retrieve this participant's enrolments
            var enrolmentResponse =
                await client.GetAsync("api/Enrolments/mine");

            if (enrolmentResponse.IsSuccessStatusCode)
            {
                var enrolments = await enrolmentResponse.Content
                    .ReadFromJsonAsync<List<EnrolmentViewModel>>();

                if (enrolments != null)
                {
                    dashboard.MyEnrolments = enrolments.Count;
                }
            }

            return View(dashboard);
        }
    }
}