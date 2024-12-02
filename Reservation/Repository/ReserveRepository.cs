using Microsoft.EntityFrameworkCore;
using Reservation.Data;
using Reservation.Data.Enum;
using Reservation.Interfaces;
using Reservation.Models;

namespace Reservation.Repository
{
    public class ReserveRepository : IReserveRepository
    {
        private readonly ApplicationDBContext _context;

        public ReserveRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<Reserve> GetReserveByIdAsync(int reserveId)
        {
            return await _context.Reserves.FirstOrDefaultAsync(c => c.ReserveId == reserveId);
        }

        public async Task<IEnumerable<Reserve>> GetReservesByUserIdAsync(string userId)
        {
            return await _context.Reserves.Where(c => c.UserId == userId).ToListAsync();
        }

        public async Task ProcessExpiredReservations()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            // Busca todas as reservas expiradas e não processadas
            var expiredReservations = await _context.Reserves.Include(r => r.ReserveEquipments).ThenInclude(re => re.Equipment).Where(r => r.ReserveDate < today && !r.Processed).ToListAsync();

            foreach (var reservation in expiredReservations)
            {
                foreach (var resEquip in reservation.ReserveEquipments)
                {
                    // Atualiza a quantidade disponível do equipamento
                    var dbEquipment = await _context.Equipments.FirstOrDefaultAsync(e => e.EquipmentId == resEquip.EquipmentId);

                    if (dbEquipment != null)
                    {
                        dbEquipment.QuantityAvailable += resEquip.Quantity;
                    }
                }

                // Marca a reserva como processada
                reservation.Processed = true;
            }

            // Salva todas as alterações no banco de dados
            await _context.SaveChangesAsync();
        }

        public async Task<Reserve> GetReserveByRoomAndDateAsync(int roomId, DateOnly reserveDate, TimeOnly reserveStart, TimeOnly reserveEnd)

        {
            return await _context.Reserves.Where(r => r.RoomId == roomId && r.ReserveDate == reserveDate)
        .Where(r => (r.ReserveStart < reserveEnd && r.ReserveEnd > reserveStart)) // Verificando se há sobreposição de horários
        .FirstOrDefaultAsync();

        }

        public async Task<IEnumerable<Reserve>> GetReserveWhereStatusIsValidAsync(string userId)
        {
            var currentDate = DateOnly.FromDateTime(DateTime.Now.Date);
            var currentTime = TimeOnly.FromDateTime(DateTime.Now);

            return await _context.Reserves
                .Where(r => r.UserId == userId &&
                            (r.ReserveStatus == EnumReserveStatus.Validated)  &&
                            (r.ReserveDate >= currentDate ||
                             (r.ReserveDate == currentDate && r.ReserveEnd > currentTime)))
                .OrderBy(r => r.ReserveDate)
                .ThenBy(r => r.ReserveStart)
                .Include(r => r.Room)
                .ToListAsync();
        }

        public async Task<IEnumerable<Reserve>> GetReservesByRoomIdAsync(int roomId)
        {
            return await _context.Reserves.Where(r => r.RoomId == roomId).ToListAsync();
        }

        public async Task<bool> UpdateReserveAsync(Reserve reserve)
        {
            _context.Reserves.Update(reserve);
            return await _context.SaveChangesAsync() > 0;
        }

        public bool AddReserve(Reserve reserve)
        {
            _context.Reserves.Add(reserve);
            return Save();
        }

        public bool UpdateReserve(Reserve reserve)
        {
            _context.Reserves.Update(reserve);
            return Save();
        }

        public bool DeleteReserve(Reserve reserve)
        {
            _context.Reserves.Remove(reserve);
            return Save();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

     
    }
}
