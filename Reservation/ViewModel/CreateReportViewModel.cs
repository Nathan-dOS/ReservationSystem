using Reservation.Data.Enum;

namespace Reservation.ViewModel
{
    public class CreateReportViewModel
    {
        public int ReportId { get; set; }
        public int ReserveId { get; set; }

        public string ReportTitle { get; set; }
        public string ReportObservation { get; set; }
        public bool ReportBanStatus { get; set; }
        public List<IFormFile> ReportFiles { get; set; } = new List<IFormFile>(); // IFormFile para fazer upload de novas imagens

    }
}
