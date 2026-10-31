using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Models;
using RaceDay.Web.Services;

namespace RaceDay.Web.Controllers;

public class AccountController : Controller
{
    private readonly RaceDayApiClient _apiClient;

    public AccountController(RaceDayApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    // Display registration page
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    // Receive registration form
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (model.Role != "Organiser" &&
            model.Role != "Participant")
        {
            ModelState.AddModelError(
                nameof(model.Role), "Invalid account role.");

            return View(model);
        }

        try
        {
            var registrationData = new
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Password = model.Password,
                PhoneNumber = model.PhoneNumber,
                Role = model.Role
            };

            using var response = await _apiClient.PostAsync(
                "api/Auth/register", registrationData);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] =
                    "Registration successful! You can now log in.";

                return RedirectToAction("Login");
            }

            ModelState.AddModelError(
                string.Empty,
                "Registration failed. Please check your details. " +
                "Your email may already be registered.");
        }
        catch (HttpRequestException)
        {
            ModelState.AddModelError(
                string.Empty,
                "Could not connect to RaceDay API. " +
                "Please make sure the API is running.");
        }

        return View(model);
    }

    
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }
}