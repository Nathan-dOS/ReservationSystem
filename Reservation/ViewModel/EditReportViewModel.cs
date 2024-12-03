using Reservation.Models;

namespace Reservation.ViewModel
{
    public class EditReportViewModel
    {
        public int ReportId { get; set; }
        public int ReserveId { get; set; }

        public string? ReportTitle { get; set; }
        public string? ReportObservation { get; set; }

        public bool ReportBanStatus { get; set; }
        

        public DateOnly? ReportDate { get; set; }
        public List<IFormFile> ReportFiles { get; set; } = new List<IFormFile>(); // IFormFile para fazer upload de novas imagens
        public List<ReportFile> ExistingFiles { get; set; } = new List<ReportFile>();


        public string? ReportCreatedBy { get; set; }


    }
}
