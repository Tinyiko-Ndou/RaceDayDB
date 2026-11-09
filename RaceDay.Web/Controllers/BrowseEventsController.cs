using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using RaceDay.Web.Models;

namespace RaceDay.Web.Controllers
{
    [Authorize(Roles = "Participant")]
    public class BrowseEventsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public BrowseEventsController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var client =
                _httpClientFactory.CreateClient("RaceDayAPI");

            var response =
                await client.GetAsync("api/Event");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Unable to load events.";

                return View(new List<EventViewModel>());
            }

            var events =
                await response.Content
                    .ReadFromJsonAsync<List<EventViewModel>>();

            events ??= new List<EventViewModel>();

            return View(events);
        }

        public async Task<IActionResult> Details(int id)
        {
            var client =
                _httpClientFactory.CreateClient("RaceDayAPI");

            var response =
                await client.GetAsync($"api/Event/{id}");

            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var eventItem =
                await response.Content
                    .ReadFromJsonAsync<EventViewModel>();

            if (eventItem == null)
            {
                return NotFound();
            }

            return View(eventItem);
        }
    }
}