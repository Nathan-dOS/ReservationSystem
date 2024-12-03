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
            if (User.IsInRole("general")) // Se for Gerente Geral exibe todos os relatórios
            {
                IEnumerable<Report> reports = await _reportRepository.GetAllReports();
                return View(reports);
            }
            if (User.IsInRole("admin")) // Se for Gerente Adminstrativo exibe apenas seus relatórios
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
                UserId = userId,
                ReportTitle = reportVM.ReportTitle,
                ReportObservation = reportVM.ReportObservation,
                ReportDate = DateOnly.FromDateTime(DateTime.Now),
                ReportCreatedBy = userEmail,
                ReportBanStatus = reportVM.ReportBanStatus,
                ReserveId = reportVM.ReserveId
            };

            if (reportVM.ReportFiles != null && reportVM.ReportFiles.Any()) // Se houver arquivos adiciona ao relatório
            {
                foreach (var file in reportVM.ReportFiles)
                {
                    if (file.Length > 0)
                    {
                        using var ms = new MemoryStream(); // Cria um MemoryStream
                        await file.CopyToAsync(ms); // Copia o arquivo para o MemoryStream
                        var archive = new ReportFile { ReportFileData = ms.ToArray(), ReportFileName = file.FileName }; // Cria um novo arquivo com os dados do MemoryStream
                        report.ReportArchives.Add(archive); // Adiciona o arquivo ao relatório
                    }
                }
            }

            _reportRepository.Add(report);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)// Esse método é tratado como se fosse um Detail
        {
            var report = await _reportRepository.GetByIdAsync(id);
            if (report == null)
            {
                return View("Error");
            }

            var reportVM = new EditReportViewModel // Ele recebe essas informações para serem exibidas na View
            {
                ReportTitle = report.ReportTitle,
                ReportObservation = report.ReportObservation,
                ReportDate = DateOnly.FromDateTime(DateTime.Now),
                ReportCreatedBy = report.ReportCreatedBy,
                ReportBanStatus = !report.ReportBanStatus,// Inverte o status do relatório, por conta do botão de Resolvido
                ReportId = report.ReportId,
                ExistingFiles = report.ReportArchives.ToList(), // Lista de arquivos armazenados
                ReserveId = report.ReserveId
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
