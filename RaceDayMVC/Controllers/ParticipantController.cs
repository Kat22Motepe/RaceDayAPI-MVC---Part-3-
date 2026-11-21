using Microsoft.AspNetCore.Mvc;
using RaceDay.MVC.Filters;

namespace RaceDay.MVC.Controllers
{
    [RoleAuthorize("Participant")]
    public class ParticipantController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
