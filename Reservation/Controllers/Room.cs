using Microsoft.AspNetCore.Mvc;

namespace Reservation.Controllers
{
    public class Room : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
