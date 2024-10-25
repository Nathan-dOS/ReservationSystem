using Microsoft.AspNetCore.Mvc;

namespace Reservation.Controllers
{
    public class EquipmentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
