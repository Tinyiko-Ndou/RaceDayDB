using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using RaceDay.Web.Models;

namespace RaceDay.Web.Controllers
{
    [Authorize(Roles = "Participant")]
    public class ResultsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ResultsController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Mine()
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

            var response =
                await client.GetAsync("api/Results/mine");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] =
                    "Unable to load your results.";

                return View(new List<ResultViewModel>());
            }

            var results =
                await response.Content
                    .ReadFromJsonAsync<List<ResultViewModel>>();

            return View(results ?? new List<ResultViewModel>());
        }
    }
}