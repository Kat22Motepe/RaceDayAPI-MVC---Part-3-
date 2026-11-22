using Microsoft.AspNetCore.Mvc;
using RaceDay.MVC.Filters;
using RaceDay.MVC.ViewModels;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace RaceDay.MVC.Controllers
{
    // Removed the class-level [RoleAuthorize("Organiser")] attribute
    public class ResultsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ResultsController(IHttpClientFactory httpClientFactory)
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

        // GET: /Results (Organiser selects an event to capture results for)
        [RoleAuthorize("Organiser")] // Added here
        public async Task<IActionResult> Index()
        {
            var client = GetAuthenticatedClient();
            var response = await client.GetAsync("/api/events/mine");

            var events = new List<EventListViewModel>();
            if (response.IsSuccessStatusCode)
            {
                var apiEvents = await response.Content.ReadFromJsonAsync<List<EventListViewModel>>();
                if (apiEvents != null) events = apiEvents;
            }

            return View(events);
        }

        // GET: /Results/Capture/5 (Organiser sees list of participants for event 5)
        [RoleAuthorize("Organiser")] // Added here
        public async Task<IActionResult> Capture(int id)
        {
            var client = GetAuthenticatedClient();

            var response = await client.GetAsync($"/api/events/{id}/enrolments");

            var enrolments = new List<ResultEntryViewModel>();
            if (response.IsSuccessStatusCode)
            {
                var apiEnrolments = await response.Content.ReadFromJsonAsync<List<EnrolmentDto>>();
                if (apiEnrolments != null)
                {
                    enrolments = apiEnrolments.Select(e => new ResultEntryViewModel
                    {
                        EnrolmentId = e.Id,
                        ParticipantName = e.ParticipantName,
                        CategoryName = e.CategoryName
                    }).ToList();
                }
            }

            ViewBag.EventId = id;
            return View(enrolments);
        }

        // POST: /Results/Capture
        [RoleAuthorize("Organiser")] // Added here
        [HttpPost]
        public async Task<IActionResult> Capture(int eventId, List<ResultEntryViewModel> results)
        {
            var client = GetAuthenticatedClient();

            foreach (var result in results)
            {
                if (result.Position.HasValue && !string.IsNullOrEmpty(result.FinishTime))
                {
                    if (TimeSpan.TryParse(result.FinishTime, out TimeSpan finishTime))
                    {
                        var payload = new
                        {
                            EnrolmentId = result.EnrolmentId,
                            FinishTime = finishTime,
                            Position = result.Position.Value
                        };

                        await client.PostAsJsonAsync("/api/results", payload);
                    }
                }
            }

            TempData["SuccessMessage"] = "Results captured successfully!";
            return RedirectToAction("Index");
        }

        // GET: /Results/MyResults (Participant view of their own results)
        [RoleAuthorize("Participant")]
        public async Task<IActionResult> MyResults()
        {
            var client = GetAuthenticatedClient();
            var response = await client.GetAsync("/api/results/mine");

            var results = new List<ResultDto>();
            if (response.IsSuccessStatusCode)
            {
                var apiResults = await response.Content.ReadFromJsonAsync<List<ResultDto>>();
                if (apiResults != null) results = apiResults;
            }

            return View(results);
        }
    }
}