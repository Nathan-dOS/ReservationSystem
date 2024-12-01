using Microsoft.AspNetCore.Mvc;
using Reservation.Data;
using Reservation.Interfaces;
using Reservation.Models;
using Reservation.Repository;

namespace Reservation.Controllers
{
    public class EquipmentController : Controller
    {
        private readonly IEquipmentRepository _equipmentRepository;

        public EquipmentController(IEquipmentRepository equipmentRepository)
        {
            _equipmentRepository = equipmentRepository;
        }
        public async Task<IActionResult> Index()
        {

            IEnumerable<Equipment> equipments = await _equipmentRepository.GetAllEquipments();

            return View(equipments);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Equipment equipment)
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }
                return View();
            }

            _equipmentRepository.Add(equipment);

            return RedirectToAction("Index");

        }

    }
}
