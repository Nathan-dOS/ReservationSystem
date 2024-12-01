using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reservation.Data.Enum;
using Reservation.Interfaces;
using Reservation.Models;
using Reservation.Repository;
using Reservation.ViewModel;

namespace Reservation.Services
{ // Classe criada para modularizar as funções do controller
    public class ReserveServicesRepository : IReserveService
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IReserveRepository _reserveRepository;
        private readonly UserManager<User> _userManagment;
        private readonly IReserveHistoryRepository _reserveHistory;

        public ReserveServicesRepository(IRoomRepository roomRepository, IReserveRepository reserveRepository, UserManager<User> userManagment, IReserveHistoryRepository reserveHistory)
        {
            _roomRepository = roomRepository;
            _reserveRepository = reserveRepository;
            _userManagment = userManagment;
            _reserveHistory = reserveHistory;
        }

        // Metodo para avaliar se usuario esta banido
        public async Task<bool> IsUserBanned(string UserID)
        {
            var user = await _userManagment.FindByIdAsync(UserID);
            // Verifica a flag banned
            if (user.IsBanned == true)
            {
                return true;
            }

            return false;


        }

        
        public bool IsValidBusinessHours(TimeOnly start, TimeOnly end)
        { // 08:00 e 20:00
            var openingTime = new TimeOnly(8, 0);
            var closingTime = new TimeOnly(20, 0);
            // Verifica se o horario nao ultrapassa o horario comercial
            return start >= openingTime && end <= closingTime;
        }

        public bool IsValidReserveDate(DateOnly reserveDate)
        {
            return reserveDate >= DateOnly.FromDateTime(DateTime.Now);
        }

        public bool IsValidReserveTime(TimeOnly start, TimeOnly end)
        { // Horario inicial precisa ser menor que horario final
            return start < end;
        }


        public async Task<Reserve?> CheckExistingReservation(CreateReserveViewModel reserveModel)
        {
            return await _reserveRepository.GetReserveByRoomAndDateAsync(
                reserveModel.RoomId,
                reserveModel.ReserveDate,
                reserveModel.ReserveStart,
                reserveModel.ReserveEnd
            );
        }

        // Calculo de preço por hora
        public float CalculatePriceByHours(TimeOnly start, TimeOnly end, float rentPrice)
        {
            double totalTime = (end - start).TotalHours;

            return (float)Math.Round(totalTime * rentPrice, 2);


        }


        public bool CreateReservation(RoomDetailViewModel reserveVM)
        {

            var reserve = new Reserve
            {
                RoomId = reserveVM.CreateReserveViewModel.RoomId,
                UserId = reserveVM.CreateReserveViewModel.UserId,
                ReserveDate = reserveVM.CreateReserveViewModel.ReserveDate,
                ReserveStart = reserveVM.CreateReserveViewModel.ReserveStart,
                ReserveEnd = reserveVM.CreateReserveViewModel.ReserveEnd,
                ReserveStatus = EnumReserveStatus.Validated,
                RentPrice = reserveVM.CreateReserveViewModel.RentPrice,
                ReserveEquipments = new List<ReserveEquipment>()
            };

            // Adiciona os equipamentos à reserva
            foreach (var equipment in reserveVM.RoomEquipments)
            {
                // Verifica se a quantidade do equipamento é maior que 0, pois só deve adicionar equipamentos com quantidade > 0
                if (equipment.EquipmentQuantity > 0)
                {
                    var reserveEquipment = new ReserveEquipment
                    {
                        EquipmentId = equipment.EquipmentId.Value, // Assume que EquipmentId não é nulo
                        ReserveId = reserve.ReserveId, // O ID gerado está disponível aqui
                        Quantity = equipment?.EquipmentQuantity ?? 0 // Assume que EquipmentQuantity não é nulo
                    };

                    reserve.ReserveEquipments.Add(reserveEquipment);
                }
            }

            bool succeed = _reserveRepository.AddReserve(reserve);

            if (succeed)
            {
                // Adicionar a reserva ao histórico simultanemanete. Tive que usar assim pq nao tava gerando o ID da reserva
                var history = new ReserveHistory
                {
                    ReserveId = reserve.ReserveId, // O ID gerado está disponível aqui
                    UserId = reserve.UserId,
                    RoomId = reserve.RoomId,
                    ReserveDate = reserve.ReserveDate,
                    ReserveStart = reserve.ReserveStart,
                    ReserveEnd = reserve.ReserveEnd,
                    ReserveStatus = reserve.ReserveStatus,
                    RentPrice = reserve.RentPrice,
                    ModifiedAt = DateTime.UtcNow
                    
                };

                return _reserveHistory.AddHistory(history);


            }
            else
            {
                return false;
            }


        }

        // Metodo que adiciona historico
        public bool AddReserveToHistoryAsync(Reserve reserve)
        {
            var history = new ReserveHistory
            {
                ReserveId = reserve.ReserveId,
                UserId = reserve.UserId,
                RoomId = reserve.RoomId,
                ReserveDate = reserve.ReserveDate,
                ReserveStart = reserve.ReserveStart,
                RentPrice = reserve.RentPrice,
                ReserveEnd = reserve.ReserveEnd,
                ReserveStatus = reserve.ReserveStatus,
                ModifiedAt = DateTime.UtcNow
            };

            return _reserveHistory.AddHistory(history);
            
        }

        // Atualiza o historico
        public async Task<bool> UpdateStatusHistory(Reserve reserve)
        {
            // Pega o historico baseado no ID da reserva
            var HistoryByReserveID = await _reserveHistory.GetHistoryByReserveIDAsync(reserve.ReserveId);

            if (HistoryByReserveID != null)
            {
                HistoryByReserveID.ReserveStatus = EnumReserveStatus.Canceled; // Muda seu status
                _reserveHistory.Update(HistoryByReserveID); 

                return true;
            }

            return false;
        }
    }
}
