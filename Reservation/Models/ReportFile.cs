using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Reservation.Models
{
    public class ReportFile
    {
        [Key]
        public int ReportFileId { get; set; }

        [Required]
        public byte[] ReportFileData { get; set; } // Dados do arquivo BLOB
        [ForeignKey("Report")]

        public int ReportId { get; set; }
        public Report Report { get; set; }
    }
}
