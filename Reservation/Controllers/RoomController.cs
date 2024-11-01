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
                IdRoom = room.IdRoom,
                Number = room.Number,
                Capacity = room.Capacity,
                Size = room.Size,
                Internet = room.Internet,
                Status = room.Status,
                Price = room.Price,
                PhotoAlbum = room.PhotoAlbum,
                SecurityCamera = room.SecurityCamera,
                AirConditioning = room.AirConditioning,
                Type = room.Type
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
                IdRoom = id,
                Number = roomVM.Number,
                Capacity = roomVM.Capacity,
                Size = roomVM.Size,
                Internet = roomVM.Internet,
                Status = roomVM.Status,
                Price = roomVM.Price,
                PhotoAlbum = roomVM.PhotoAlbum,
                SecurityCamera = roomVM.SecurityCamera,
                AirConditioning = roomVM.AirConditioning,
                Type = roomVM.Type
            };

            _roomRepository.Update(room);

            return RedirectToAction("Index");
        }
    }
}
