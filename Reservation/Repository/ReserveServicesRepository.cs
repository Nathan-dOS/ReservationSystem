using Reservation.Interfaces;
using Reservation.Models;
using Reservation.ViewModel;

namespace Reservation.Repository
{
    public class ReserveServicesRepository : IReserveService
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IReserveRepository _reserveRepository;

        public ReserveServicesRepository(IRoomRepository roomRepository, IReserveRepository reserveRepository)
        {
            _roomRepository = roomRepository;
            _reserveRepository = reserveRepository;
            
        }

        public bool IsValidBusinessHours(TimeOnly start, TimeOnly end)
        {
            var openingTime = new TimeOnly(8, 0);
            var closingTime = new TimeOnly(20, 0);
            // Verifica se o horario nao ultrapassa o horario comercial
            return (start >= openingTime && end <= closingTime);
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

        public bool CreateReservation(CreateReserveViewModel reserveModel)
        {
            var reserve = new Reserve
            {
                RoomId = reserveModel.RoomId,
                UserId = reserveModel.UserId,
                ReserveDate = reserveModel.ReserveDate,
                ReserveStart = reserveModel.ReserveStart,
                ReserveEnd = reserveModel.ReserveEnd,
                ReserveStatus = reserveModel.ReserveStatus,
                RentPrice = reserveModel.RentPrice,
            };

            return _reserveRepository.AddReserve(reserve);

        }
    }
}
