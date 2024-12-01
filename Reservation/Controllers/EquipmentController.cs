using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reservation.Data;
using Reservation.Interfaces;
using Reservation.Models;
using Reservation.Repository;
using Reservation.ViewModel;

namespace Reservation.Controllers
{
    public class EquipmentController : Controller
    {
        private readonly IEquipmentRepository _equipmentRepository;

        public EquipmentController(IEquipmentRepository equipmentRepository)
        {
            _equipmentRepository = equipmentRepository;
        }
        [Authorize]
        public async Task<IActionResult> Index()
        {

            IEnumerable<Equipment> equipments = await _equipmentRepository.GetAllEquipments();

            return View(equipments);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(CreateEquipmentViewModel equipmentViewModel)
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }
                return View();
            }

            var equipment = new Equipment
            {
                EquipmentId = equipmentViewModel.EquipmentId,
                EquipmentName = equipmentViewModel.EquipmentName,
                EquipmentPrice = equipmentViewModel.EquipmentPrice,
                QuantityAvailable = equipmentViewModel.QuantityAvailable
            };

            _equipmentRepository.Add(equipment);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var equipment = await _equipmentRepository.GetEquipmentById(id);
            if (equipment == null)
            {
                return NotFound();
            }

            var equipmentViewModel = new EditEquipmentViewModel
            {
                EquipmentId = equipment.EquipmentId,
                EquipmentName = equipment.EquipmentName,
                EquipmentPrice = equipment.EquipmentPrice,
                QuantityAvailable = equipment.QuantityAvailable
            };

            return View(equipmentViewModel);
        }

        [HttpPost]
        public IActionResult Edit(EditEquipmentViewModel equipmentViewModel)
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }
                return View();
            }
            var equipment = new Equipment
            {
                EquipmentId = equipmentViewModel.EquipmentId,
                EquipmentName = equipmentViewModel.EquipmentName,
                EquipmentPrice = equipmentViewModel.EquipmentPrice,
                QuantityAvailable = equipmentViewModel.QuantityAvailable
            };
            _equipmentRepository.Update(equipment);

            return RedirectToAction("Index");
        }


    }
}
