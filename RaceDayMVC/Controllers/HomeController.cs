using Microsoft.AspNetCore.Mvc;

namespace RaceDay.MVC.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // If user is logged in, redirect to their specific dashboard
            var role = HttpContext.Session.GetString("UserRole");
            if (role == "Organiser") return RedirectToAction("Dashboard", "Organiser");
            if (role == "Participant") return RedirectToAction("Index", "Participant");

            // If not logged in, show the default landing page
            return View();
        }
    }
}