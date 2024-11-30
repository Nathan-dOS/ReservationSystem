using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Reservation.Interfaces;
using Reservation.Models;
using Reservation.Repository;
using Reservation.ViewModel;
using System.Security.Claims;

namespace Reservation.Controllers
{
    public class ReserveController : Controller
    {
        private readonly IReserveRepository _reserveRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly IReserveService _reserveService;
        public ReserveController(IReserveRepository reserveRepository, IRoomRepository roomRepository, IReserveService reserveService)
        {
            _reserveRepository = reserveRepository;
            _roomRepository = roomRepository;
            _reserveService = reserveService;

        }

        [Authorize]
        public IActionResult Create(int roomId)
        {

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (roomId == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var reserveVM = new CreateReserveViewModel
            {
                RoomId = roomId,
                UserId = userId,
            };

            return View(reserveVM);
        }

        [HttpPost]
        public async Task<IActionResult> Create(RoomDetailViewModel roomDetail)
        {
            if (!ModelState.IsValid)
            {
                // Se o modelo não for válido, obteremos os detalhes da sala e passaremos para a view novamente
                var room = await _roomRepository.GetByIdAsync(roomDetail.CreateReserveViewModel.RoomId);


                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }


                if (room == null)
                {
                    return View("Error"); // Se a sala não existir, mostra um erro
                }


                var user = roomDetail.CreateReserveViewModel.UserId;

                // Passa os detalhes da sala junto com o modelo de reserva de volta para a View de detalhes
                var roomDetailViewModel = new RoomDetailViewModel
                {
                    Room = room,
                    CreateReserveViewModel = roomDetail.CreateReserveViewModel,

                };

                // Exibe a página de detalhes com os erros de validação
                TempData["ErrorMessage"] = "Model State Invalido";
                return RedirectToAction("Detail", "Room", new { id = roomDetail.CreateReserveViewModel.RoomId });
            }

            // Criei um reserveService para lidar com possíveis calculos/erros/validações que nao exigem buscar no banco de dados (No qual apenas o ReserveRepository faz essa função).
            // Para fazer, implementei uma interface e criei um reserveService onde contem todos as operaçõeos


            if (await _reserveService.IsUserBanned(roomDetail.CreateReserveViewModel.UserId))
            {
                TempData["ErrorMessage"] = "Você está banido de fazer reserva por enquanto";
                return RedirectToAction("BanView", "UserManagment");
            }

            if (!_reserveService.IsValidBusinessHours(roomDetail.CreateReserveViewModel.ReserveStart, roomDetail.CreateReserveViewModel.ReserveEnd))
            {
                TempData["ErrorMessage"] = "Os horários de reserva devem estar entre 08:00 e 20:00.";
                return RedirectToAction("Detail", "Room", new { id = roomDetail.CreateReserveViewModel.RoomId });

            }


            if (!_reserveService.IsValidReserveTime(roomDetail.CreateReserveViewModel.ReserveStart, roomDetail.CreateReserveViewModel.ReserveEnd))
            {
                TempData["ErrorMessage"] = "O horario de inicio deve ser anterior ao horario do final. ";
                return RedirectToAction("Detail", "Room", new { id = roomDetail.CreateReserveViewModel.RoomId });

            }


            if (!_reserveService.IsValidReserveDate(roomDetail.CreateReserveViewModel.ReserveDate))
            {

                TempData["ErrorMessage"] = "Selecione um dia válido";
                return RedirectToAction("Detail", "Room", new { id = roomDetail.CreateReserveViewModel.RoomId });

            }


            var existingReserve = await _reserveService.CheckExistingReservation(roomDetail.CreateReserveViewModel);

            if (existingReserve != null) // Modifiquei aqui, inclui o TempData ao inves do AddModelErro. (Até funcionava, mas nao tava exibindo msg pro usuario)
            {

                // ModelState.AddModelError("CreateReserveViewModel.ReserveDate", "Já existe uma reserva para esta sala neste horário.");
                TempData["ErrorMessage"] = "Já existe uma reserva para esta sala neste horário.";
                return RedirectToAction("Detail", "Room", new { id = roomDetail.CreateReserveViewModel.RoomId });
            }


            float totalPriceByHours = _reserveService.CalculatePriceByHours(roomDetail.CreateReserveViewModel.ReserveStart, roomDetail.CreateReserveViewModel.ReserveEnd,
               roomDetail.CreateReserveViewModel.RentPrice);

            // CHEIRO DE GAMBIARRA ESSE TotalPriceByHours, DEPOIS ALTERAR
            _reserveService.CreateReservation(roomDetail.CreateReserveViewModel, totalPriceByHours);


            return RedirectToAction("Confirmation", "Reserve");


        }

        public IActionResult Confirmation()
        {



            return View();
        }

        public async Task<IActionResult> History()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Obtém o ID do usuário logado
            if (string.IsNullOrEmpty(userId))
            {
                TempData["ErrorMessage"] = "Usuário não autenticado.";
                return RedirectToAction("Login", "Account");
            }

            var reservations = await _reserveRepository.GetReservesByUserIdAsync(userId);

            var historyViewModel = reservations.Select(r => new ReserveHistoryViewModel
            {
                ReserveId = r.ReserveId,
                UserId = r.UserId,
                EquipementId = r.EquipementId,
                ReserveDate = r.ReserveDate,
                ReserveStart = r.ReserveStart,
                ReserveEnd = r.ReserveEnd,
                RentPrice = r.RentPrice,
                ReserveStatus = r.ReserveStatus.ToString()
            });

            return View(historyViewModel);
        }

        public async Task<IActionResult> CancelReserve(int id)
        {
            var reserveID = await _reserveRepository.GetReserveByIdAsync(id);

            if (reserveID == null)
            {
                TempData["ErrorMessage"] = "Reserva Inválida";
                return RedirectToAction("MyReserves", "UserManagment");

            }

            return View(reserveID);
        }


        public async Task<IActionResult> UserReserves()
        {
            var userID = User.FindFirstValue(ClaimTypes.NameIdentifier); // Obtém o ID do usuário logado

            if (userID == null)
            {
                TempData["ErrorMessage"] = "Usuario Inválido";
                RedirectToAction("Index");
            }

            var reserveByUser = await _reserveRepository.GetReserveWhereStatusIsValidAsync(userID);

            return View(reserveByUser);
           


        }
    }

}
