using Reservation.Models;

namespace Reservation.Interfaces
{
    public interface IReportRepository
    {
        Task<IEnumerable<Report>> GetAllReports();
        Task<Report> GetByIdAsync(int id);
        bool Add(Report report);
        bool Update(Report report);
        bool Delete(Report report);
        Task<List<Report>> GetReportsByUserIdAsync(string userId);
        Task<ReportFile> GetFileByIdAsync(int fileId);
        bool Save();
    }
}
