using Microsoft.AspNetCore.Mvc;
using Reservation.Data;
using Reservation.Models;

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

        public IActionResult Detail(int id)
        {
            Room roomID = _context.Rooms.FirstOrDefault(c => c.IdRoom == id);

            return View(roomID);
        }
    }
}
