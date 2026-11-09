using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using RaceDay.Web.Models;

namespace RaceDay.Web.Controllers
{
    [Authorize(Roles = "Organiser")]
    public class OrganiserController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public OrganiserController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Dashboard()
        {
            var client = _httpClientFactory.CreateClient("RaceDayAPI");

            var token = HttpContext.Session.GetString("JWTToken");

            if (!string.IsNullOrWhiteSpace(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await client.GetAsync("api/Event");

            var events = response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<List<EventViewModel>>()
                : new List<EventViewModel>();

            events ??= new List<EventViewModel>();

            var model = new OrganiserDashboardViewModel
            {
                TotalEvents = events.Count,
                UpcomingEvents = events.Count(e =>
                    e.Status.Equals("Upcoming",
                    StringComparison.OrdinalIgnoreCase)),
                CompletedEvents = events.Count(e =>
                    e.Status.Equals("Completed",
                    StringComparison.OrdinalIgnoreCase))
            };

            return View(model);
        }
    }
}
