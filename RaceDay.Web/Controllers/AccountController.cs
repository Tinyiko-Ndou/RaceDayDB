using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Models;
using RaceDay.Web.Services;
using System.Net.Http.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var loginData = new
            {
                Email = model.Email,
                Password = model.Password
            };

            using var response = await _apiClient.PostAsync(
                "api/Auth/login", loginData);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Invalid email address or password.");

                return View(model);
            }

            var result = await response.Content
                .ReadFromJsonAsync<LoginApiResponse>();

            if (result == null ||
                string.IsNullOrWhiteSpace(result.Token) ||
                result.User == null ||
                string.IsNullOrWhiteSpace(result.User.Email))
            {
                ModelState.AddModelError(
                    string.Empty,
                    "The API returned an incomplete login response.");

                return View(model);
            }

            var role = result.User.Role;

            if (role != "Organiser" && role != "Participant")
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Your account has an invalid role.");

                return View(model);
            }

            // Store the API token on the server-side session.
            HttpContext.Session.SetString("JWTToken", result.Token);

            // Create claims for the MVC authentication cookie.
            var claims = new List<Claim>
        {
            //new Claim(
            //    ClaimTypes.NameIdentifier,
             //   result.User.UserId.ToString()),

            new Claim(
                ClaimTypes.Name,
                result.User.Email),

            new Claim(
                ClaimTypes.Email,
                result.User.Email),

            new Claim(
                ClaimTypes.Role,
                role),

            new Claim(
                "FirstName",
                result.User.FirstName ?? ""),

            new Claim(
                "LastName",
                result.User.LastName ?? "")
        };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal);

            TempData["SuccessMessage"] = "Login successful!";

            if (role == "Organiser")
            {
                return RedirectToAction(
                    "Dashboard", "Organiser");
            }

            return RedirectToAction(
                "Dashboard", "Participant");
        }
        catch (HttpRequestException)
        {
            ModelState.AddModelError(
                string.Empty,
                "Could not connect to RaceDay API. Please try again.");
        }

        return View(model);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);

        HttpContext.Session.Clear();

        return RedirectToAction("Login");
    }

}