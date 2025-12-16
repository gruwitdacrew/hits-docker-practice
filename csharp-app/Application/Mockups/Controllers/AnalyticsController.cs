using Microsoft.AspNetCore.Mvc;

namespace Mockups.Controllers
{
    public class AnalyticsController : Controller
    {
        public IActionResult Index()
        {
            // This could be a page that shows non-sensitive analytics to regular users
            // or simply redirect to admin analytics if user is admin
            return RedirectToAction("Index", "Menu");
        }
    }
}