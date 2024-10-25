using Microsoft.AspNetCore.Mvc;
using Reservation.Data;

namespace Reservation.Controllers
{
    public class RoomController : Controller
    {
        private readonly ApplicationDBContext _context;

        public RoomController(ApplicationDBContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var rooms = _context.Rooms.ToList();

            return View(rooms);
        }
    }
}
