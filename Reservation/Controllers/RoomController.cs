using Microsoft.AspNetCore.Mvc;
using Reservation.Data;
using Reservation.Interfaces;
using Reservation.Models;

namespace Reservation.Controllers
{
    public class RoomController : Controller
    {
        private readonly IRoomRepository _roomRepository;

        public RoomController(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }

        public async Task<IActionResult> Index()
        {
           IEnumerable<Room> rooms = await _roomRepository.GetAllRooms();

            return View(rooms);
        }

        public async Task<IActionResult> Detail(int id)
        {
            Room roomID = await _roomRepository.GetByIdAsync(id);

            return View(roomID);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Room room)
        {
            if (!ModelState.IsValid)
            {
                return View(room);
            }
            _roomRepository.Add(room);

            return RedirectToAction("Index");

        }
    }
}
