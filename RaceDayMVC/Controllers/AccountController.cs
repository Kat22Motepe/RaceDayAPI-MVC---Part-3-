using Microsoft.AspNetCore.Mvc;
using RaceDay.MVC.ViewModels;
using System.Net.Http.Json;

namespace RaceDay.MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AccountController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("UserRole") != null)
                return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var client = _httpClientFactory.CreateClient("RaceDayAPI");

            // REMOVED the "dto" wrapper. Sending flat JSON.
            var loginPayload = new
            {
                Email = model.Email,
                Password = model.Password
            };

            // Matches UsersController: [HttpPost("auth/login")]
            var response = await client.PostAsJsonAsync("/api/auth/login", loginPayload);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiLoginResponse>();

                if (result != null)
                {
                    HttpContext.Session.SetString("UserRole", result.Role);
                    HttpContext.Session.SetString("UserName", result.FirstName);
                    HttpContext.Session.SetString("ApiToken", result.Token);

                    if (result.Role == "Organiser" || result.Role == "1")
                        return RedirectToAction("Dashboard", "Organiser");
                    else
                        return RedirectToAction("Index", "Participant");
                }
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt. Please check your credentials.");
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var client = _httpClientFactory.CreateClient("RaceDayAPI");

            // Convert string to the Enum integer (Organiser = 1, Participant = 2)
            int roleEnumValue = model.Role == "Organiser" ? 1 : 2;

            // REMOVED the "dto" wrapper. Sending flat JSON.
            var registerPayload = new
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Password = model.Password,
                Role = roleEnumValue
            };

            // Matches UsersController: [HttpPost("auth/register")]
            var response = await client.PostAsJsonAsync("/api/auth/register", registerPayload);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Registration successful! Please log in.";
                return RedirectToAction("Login");
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, $"Registration failed: {errorContent}");

            return View(model);
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}