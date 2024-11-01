using Microsoft.AspNetCore.Mvc;
using Reservation.Data;
using Reservation.Interfaces;
using Reservation.Models;
using Reservation.ViewModel;
using System.Drawing;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var room = await _roomRepository.GetByIdAsync(id);
            if (room == null)
            {
                return View("Error");
            }

            var roomVM = new EditRoomViewModel
            {
                RoomId = room.RoomId,
                RoomNumber = room.RoomNumber,
                Capacity = room.Capacity,
                SizeInSquareMeters = room.SizeInSquareMeters,
                HasInternet = room.HasInternet,
                RoomStatus = room.RoomStatus,
                RoomPrice = room.RoomPrice,
                PhotoAlbum = room.PhotoAlbum,
                HasSecurityCamera = room.HasSecurityCamera,
                HasAirConditioning = room.HasAirConditioning,
                RoomType = room.RoomType
            };

            return View(roomVM);

        }
        [HttpPost]
        public IActionResult Edit(int id, EditRoomViewModel roomVM)
        {
            if(!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Failed to edit Room");
                return View("Edit", roomVM);
            }

            var room = new Room
            {
                RoomId = id,
                RoomNumber = roomVM.RoomNumber,
                Capacity = roomVM.Capacity,
                SizeInSquareMeters = roomVM.SizeInSquareMeters,
                HasInternet = roomVM.HasInternet,
                RoomStatus = roomVM.RoomStatus,
                RoomPrice = roomVM.RoomPrice,
                PhotoAlbum = roomVM.PhotoAlbum,
                HasSecurityCamera = roomVM.HasSecurityCamera,
                HasAirConditioning = roomVM.HasAirConditioning,
                RoomType = roomVM.RoomType
            };

            _roomRepository.Update(room);

            return RedirectToAction("Index");
        }
    }
}
