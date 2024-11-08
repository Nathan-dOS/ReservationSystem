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
        public async Task<IActionResult> Create(CreateRoomViewModel roomVM)
        {
            if (!ModelState.IsValid) return View(roomVM);

            var room = new Room
            {
                RoomNumber = roomVM.RoomNumber,
                Capacity = roomVM.Capacity,
                SizeInSquareMeters = roomVM.SizeInSquareMeters,
                RoomStatus = roomVM.RoomStatus,
                RoomPrice = roomVM.RoomPrice,
                HasInternet = roomVM.HasInternet,
                HasSecurityCamera = roomVM.HasSecurityCamera,
                HasAirConditioning = roomVM.HasAirConditioning,
                RoomType = roomVM.RoomType
            };

            if (roomVM.RoomImages != null && roomVM.RoomImages.Any())// se houver imagens as adiciona a tabela
            {
                foreach (var file in roomVM.RoomImages)
                {
                    if (file.Length > 0)
                    {
                        using var ms = new MemoryStream();// cria um novo MemoryStream
                        await file.CopyToAsync(ms);// copia o arquivo para o MemoryStream
                        var image = new RoomImage { ImageData = ms.ToArray() };// cria um novo objeto RoomImage e armazena o array de bytes
                        room.PhotoAlbum.Add(image);// adiciona a imagem a lista de imagens do quarto
                    }
                }
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
                ExistingImages = room.PhotoAlbum.Select(img => img.ImageData).ToList(),// lista de imagens ja armazenadas
                HasSecurityCamera = room.HasSecurityCamera,
                HasAirConditioning = room.HasAirConditioning,
                RoomType = room.RoomType
            };

            return View(roomVM);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, EditRoomViewModel roomVM)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Failed to edit Room");
                return View("Edit", roomVM);
            }

            var room = await _roomRepository.GetByIdAsync(id);
            if (room == null) return View("Error");

            room.RoomNumber = roomVM.RoomNumber;
            room.Capacity = roomVM.Capacity;
            room.SizeInSquareMeters = roomVM.SizeInSquareMeters;
            room.HasInternet = roomVM.HasInternet;
            room.RoomStatus = roomVM.RoomStatus;
            room.RoomPrice = roomVM.RoomPrice;
            room.HasSecurityCamera = roomVM.HasSecurityCamera;
            room.HasAirConditioning = roomVM.HasAirConditioning;
            room.RoomType = roomVM.RoomType;

            if (roomVM.DeleteImages != null && roomVM.DeleteImages.Any())// se houver imagens a serem deletadas as remove da tabela
            {
                foreach (var index in roomVM.DeleteImages.OrderByDescending(i => i))
                {
                    if (index >= 0 && index < room.PhotoAlbum.Count)
                    {
                        room.PhotoAlbum.RemoveAt(index);
                    }
                }
            }

            if (roomVM.RoomImages != null && roomVM.RoomImages.Any()) // se houver imagens as adiciona a tabela
            {
                foreach (var file in roomVM.RoomImages)
                {
                    if (file.Length > 0)
                    {
                        using var ms = new MemoryStream();
                        await file.CopyToAsync(ms);
                        var image = new RoomImage { ImageData = ms.ToArray() };
                        room.PhotoAlbum.Add(image);
                    }
                }
            }

            _roomRepository.Update(room);
            return RedirectToAction("Index");
        }
    }
}
