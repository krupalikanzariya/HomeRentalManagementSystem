using Microsoft.AspNetCore.Mvc;

namespace HomeRentalFrontEnd.Controllers
{
    [CheckAccess]
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
