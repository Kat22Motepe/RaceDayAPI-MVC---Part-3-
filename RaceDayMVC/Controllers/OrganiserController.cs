using Microsoft.AspNetCore.Mvc;
using RaceDay.MVC.Filters;
using RaceDay.MVC.ViewModels;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace RaceDay.MVC.Controllers
{
    [RoleAuthorize("Organiser")]
    public class OrganiserController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public OrganiserController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private HttpClient GetAuthenticatedClient()
        {
            var client = _httpClientFactory.CreateClient("RaceDayAPI");
            var token = HttpContext.Session.GetString("ApiToken");
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            return client;
        }

        public async Task<IActionResult> Dashboard()
        {
            // Get events for this organiser from the API
            var client = GetAuthenticatedClient();
            var response = await client.GetAsync("/api/events/mine");

            if (response.IsSuccessStatusCode)
            {
                var events = await response.Content.ReadFromJsonAsync<List<object>>();
                return View(events); // We will just pass dynamic objects for now
            }

            return View(new List<object>());
        }

        [HttpGet]
        public IActionResult CreateEvent()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateEvent(CreateEventViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var client = GetAuthenticatedClient();

            // Create multipart form data
            var formData = new MultipartFormDataContent();
            formData.Add(new StringContent(model.Name), "Name");
            formData.Add(new StringContent(model.Description), "Description");
            formData.Add(new StringContent(model.EventDate.ToString("yyyy-MM-ddTHH:mm:ss")), "EventDate");
            formData.Add(new StringContent(model.Location), "Location");
            formData.Add(new StringContent(model.DistanceKm.ToString()), "DistanceKm");
            formData.Add(new StringContent(((int)model.EventType).ToString()), "EventType");

            if (model.BannerImage != null && model.BannerImage.Length > 0)
            {
                var fileContent = new StreamContent(model.BannerImage.OpenReadStream());
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(model.BannerImage.ContentType);
                formData.Add(fileContent, "bannerImage", model.BannerImage.FileName);
            }

            var response = await client.PostAsync("/api/events", formData);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Event created successfully!";
                return RedirectToAction("Dashboard");
            }

            var error = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", $"API Error: {error}");
            return View(model);
        }
    }
}