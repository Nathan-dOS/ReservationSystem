using Reservation.Models;
using Reservation.Repository;

namespace Reservation.Interfaces
{
    public interface IReserveHistoryRepository
    {
        bool AddHistory(ReserveHistory reserveHistory);

        bool Save();

        Task<ReserveHistory> GetHistoryByReserveIDAsync(int reserveID);

        Task<IEnumerable<ReserveHistory>> GetHistoryByUserIDAsync(string userID);

        bool Update(ReserveHistory reserveHistory); 
    }
}
