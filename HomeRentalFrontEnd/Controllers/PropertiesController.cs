using Microsoft.AspNetCore.Mvc;

namespace HomeRentalFrontEnd.Controllers
{
    public class PropertiesController : Controller
    {
        public IActionResult PropertiesList()
        {
            return View();
        }
    }
}
