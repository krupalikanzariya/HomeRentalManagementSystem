using Microsoft.AspNetCore.Mvc;

namespace HomeRentalFrontEnd.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
