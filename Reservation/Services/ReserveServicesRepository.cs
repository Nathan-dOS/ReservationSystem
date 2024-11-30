using Microsoft.AspNetCore.Identity;
using Reservation.Interfaces;
using Reservation.Models;
using Reservation.ViewModel;

namespace Reservation.Services
{
    public class ReserveServicesRepository : IReserveService
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IReserveRepository _reserveRepository;
        private readonly UserManager<User> _userManagment;

        public ReserveServicesRepository(IRoomRepository roomRepository, IReserveRepository reserveRepository, UserManager<User> userManagment)
        {
            _roomRepository = roomRepository;
            _reserveRepository = reserveRepository;
            _userManagment = userManagment;

        }

        public async Task<bool> IsUserBanned (string UserID)
        {
            var user = await _userManagment.FindByIdAsync(UserID);

            if (user.IsBanned == true)
            {
                return true;
            }

            return false;
            

        }

        public bool IsValidBusinessHours(TimeOnly start, TimeOnly end)
        {
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
        {
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


        public float CalculatePriceByHours(TimeOnly start, TimeOnly end, float rentPrice)
        {
            double totalTime = (end - start).TotalHours;

            return (float)Math.Round(totalTime * rentPrice, 2);


        }


        public bool CreateReservation(CreateReserveViewModel reserveModel, float totalPriceByHours)
        {

            var reserve = new Reserve
            {
                RoomId = reserveModel.RoomId,
                UserId = reserveModel.UserId,
                ReserveDate = reserveModel.ReserveDate,
                ReserveStart = reserveModel.ReserveStart,
                ReserveEnd = reserveModel.ReserveEnd,
                ReserveStatus = reserveModel.ReserveStatus,
                RentPrice = totalPriceByHours,
            };

            return _reserveRepository.AddReserve(reserve);

        }
    }
}
