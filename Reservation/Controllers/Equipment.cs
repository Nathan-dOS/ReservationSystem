using Microsoft.AspNetCore.Mvc;

namespace Reservation.Controllers
{
    public class Equipment : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
