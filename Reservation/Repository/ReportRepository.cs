using Microsoft.EntityFrameworkCore;
using Reservation.Data;
using Reservation.Interfaces;
using Reservation.Models;

namespace Reservation.Repository
{
    public class ReportRepository : IReportRepository
    {
        private readonly ApplicationDBContext _context;

        public ReportRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public bool Add(Report report)
        {
            _context.Reports.Add(report);
            return Save();
        }


        public bool Update(Report report)
        {
            _context.Update(report);
            return Save();
        }

        public bool Delete(Report report)
        {
            _context.Remove(report);
            return Save();
        }
        public async Task<List<Report>> GetReportsByUserIdAsync(string userId)
        {
            return await _context.Reports.Where(r => r.UserId == userId).ToListAsync();
        }

        public async Task<IEnumerable<Report>> GetAllReports()
        {
            return await _context.Reports
                .Include(r => r.ReportArchives) // inclui a lista de arquivos
                .ToListAsync();
        }

        public async Task<Report> GetByIdAsync(int id)
        {
            return await _context.Reports
                .Include(r => r.ReportArchives) // inclui a lista de arquivos
                .FirstOrDefaultAsync(i => i.ReportId == id);
        }

        public async Task<ReportFile> GetFileByIdAsync(int fileId)
        {
            return await _context.ReportFiles.FindAsync(fileId);
        }


        public bool Save()
        {
            var saved = _context.SaveChanges();

            return saved > 0 ? true : false;
        }
    }
}
