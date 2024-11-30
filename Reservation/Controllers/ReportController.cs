using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Reservation.Interfaces;
using Reservation.Models;
using Reservation.Repository;
using Reservation.ViewModel;

namespace Reservation.Controllers
{
    public class ReportController : Controller
    {
        private readonly IReportRepository _reportRepository;
        private readonly UserManager<User> _userManager;

        public ReportController(IReportRepository reportRepository, UserManager<User> userManager)
        {
            _reportRepository = reportRepository;
            _userManager = userManager;
        }

        [Authorize] // Precisa esta logado para acessar Report
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("general")) // Se for general
            {
                IEnumerable<Report> reports = await _reportRepository.GetAllReports();
                return View(reports);
            }
            if (User.IsInRole("admin")) // Se for admin
            {
                var userId = _userManager.GetUserId(User);// Obtém o ID do usuário logado

                var userReports = await _reportRepository.GetReportsByUserIdAsync(userId);// Busca relatórios específicos do usuário

                return View(userReports);// Passa os relatórios para a View
            }
            return View("Error"); // Retorna um erro se não for admin ou general
        }
        public async Task<IActionResult> Detail(int id)
        {
            Report reportID = await _reportRepository.GetByIdAsync(id);
            return View(reportID);
        }

        [Authorize(Roles = "admin")] // Precisa ser Admin  
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "admin")] // Precisa ser Admin  

        public async Task<IActionResult> Create(CreateReportViewModel reportVM)
        {
            if (!ModelState.IsValid) return View(reportVM);

            var user = await _userManager.GetUserAsync(User); // Pega o usuário logado
            var userId = _userManager.GetUserId(User); // Pega o ID do usuário logado
            var userEmail = await _userManager.GetEmailAsync(user); // Pega o email do usuário logado

            var report = new Report
            {
                UserId = userId, // Set the required UserId property
                ReportTitle = reportVM.ReportTitle,
                ReportObservation = reportVM.ReportObservation,
                ReportDate = DateOnly.FromDateTime(DateTime.Now),
                ReportCreatedBy = userEmail,
                ReportBanStatus = reportVM.ReportBanStatus
            };

            if (reportVM.ReportFiles != null && reportVM.ReportFiles.Any()) // if there are images, add them to the table
            {
                foreach (var file in reportVM.ReportFiles)
                {
                    if (file.Length > 0)
                    {
                        using var ms = new MemoryStream(); // create a new MemoryStream
                        await file.CopyToAsync(ms); // copy the file to the MemoryStream
                        var archive = new ReportFile { ReportFileData = ms.ToArray(), ReportFileName = file.FileName }; // create a new ReportImage object and store the byte array
                        report.ReportArchives.Add(archive); // add the image to the report's photo album
                    }
                }
            }

            _reportRepository.Add(report);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var report = await _reportRepository.GetByIdAsync(id);
            if (report == null)
            {
                return View("Error");
            }

            var reportVM = new EditReportViewModel
            {
                ReportTitle = report.ReportTitle,
                ReportObservation = report.ReportObservation,
                ReportDate = DateOnly.FromDateTime(DateTime.Now),
                ReportCreatedBy = report.ReportCreatedBy,
                ReportBanStatus = !report.ReportBanStatus,
                ReportId = report.ReportId,
                ExistingFiles = report.ReportArchives.ToList() // Lista de arquivos armazenados
            };

            return View(reportVM);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, EditReportViewModel reportVM)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Failed to edit Report");
                return View("Edit", reportVM);
            }

            var report = await _reportRepository.GetByIdAsync(id);
            if (report == null) return View("Error");

            report.ReportBanStatus = !reportVM.ReportBanStatus;


            _reportRepository.Update(report);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> DownloadFile(int fileId)
        {
            var file = await _reportRepository.GetFileByIdAsync(fileId); // Método para buscar o arquivo pelo ID
            if (file == null)
            {
                return NotFound();
            }

            // Convert byte array to string for Path.GetExtension
            string fileExtension = Path.GetExtension(Convert.ToBase64String(file.ReportFileData));

            string contentType = "application/octet-stream"; // Tipo genérico de conteúdo

            
            if (fileExtension == ".pdf")
                contentType = "application/pdf";
            else if (fileExtension == ".jpg" || fileExtension == ".jpeg")
                contentType = "image/jpeg";
            else if (fileExtension == ".png")
                contentType = "image/png";

            string fileName = $"file_{fileId}{fileExtension}"; // Nome do arquivo com extensão

            return File(file.ReportFileData, contentType, fileName);
        }
    }
}
