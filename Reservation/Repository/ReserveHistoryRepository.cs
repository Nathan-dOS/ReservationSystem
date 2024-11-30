using Microsoft.EntityFrameworkCore;
using Reservation.Data;
using Reservation.Models;
using Reservation.Interfaces;

namespace Reservation.Repository
{
    public class ReserveHistoryRepository : IReserveHistoryRepository
    {
        private readonly ApplicationDBContext _dbContext;
        public ReserveHistoryRepository(ApplicationDBContext context)
        {
            _dbContext = context;
        }

        public bool AddHistory(ReserveHistory reserveHistory)
        {
           _dbContext.Add(reserveHistory);
            return Save();
        }
        public bool Save()
        {
            var saved = _dbContext.SaveChanges();
            return saved > 0 ? true : false;
        }

        public async Task<IEnumerable<ReserveHistory>> GetHistoryByUserID(string userID)
        {
           return await _dbContext.ReserveHistories.Where(r => r.UserId ==  userID).ToListAsync();
        }

    }
}
