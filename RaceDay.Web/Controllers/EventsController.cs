using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using RaceDay.Web.Models;

namespace RaceDay.Web.Controllers
{
    [Authorize(Roles = "Organiser")]
    public class EventsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public EventsController(IHttpClientFactory httpClientFactory)
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

        public async Task<IActionResult> Index()
        {
            var client = CreateApiClient();

            var response =
                await client.GetAsync("api/Event");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Could not load events.";
                return View(new List<EventViewModel>());
            }

            var events =
                await response.Content.ReadFromJsonAsync<List<EventViewModel>>();

            return View(events ?? new List<EventViewModel>());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateEventViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var client = CreateApiClient();

            var response =
     await client.PostAsJsonAsync("api/Event", model);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Event created successfully.";
                return RedirectToAction(nameof(Index));
            }

            
            var errorMessage =
                await response.Content.ReadAsStringAsync();

            TempData["Error"] =
                $"API Error: {(int)response.StatusCode} " +
                $"{response.StatusCode}. Details: {errorMessage}";

            ModelState.AddModelError(
                "",
                TempData["Error"].ToString()!
            );

            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var client = CreateApiClient();

            var response =
                await client.GetAsync($"api/Event/{id}");

            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var eventItem =
                await response.Content.ReadFromJsonAsync<EventViewModel>();

            if (eventItem == null)
            {
                return NotFound();
            }

            var model = new EditEventViewModel
            {
                EventID = eventItem.EventID,
                EventName = eventItem.EventName,
                Description = eventItem.Description,
                EventType = eventItem.EventType,
                EventDate = eventItem.EventDate,
                StartTime = eventItem.StartTime,
                Location = eventItem.Location,
                City = eventItem.City,
                Province = eventItem.Province,
                Status = eventItem.Status
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            EditEventViewModel model)
        {
            if (id != model.EventID)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var client = CreateApiClient();

            var response =
                await client.PutAsJsonAsync($"api/Event/{id}", model);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Event updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(
                "",
                "The API could not update the event."
            );

            return View(model);
        }

        public IActionResult Delete(int id)
        {
            return View(id);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = CreateApiClient();

            var response =
                await client.DeleteAsync($"api/Event/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Event deleted successfully.";
            }
            else
            {
                TempData["Error"] = "The event could not be deleted.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}