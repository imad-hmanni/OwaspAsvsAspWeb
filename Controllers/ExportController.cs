using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplicationAsp.Entities;
using WebApplicationAsp.Repository;
using WebApplicationAsp.Services;

namespace WebApplicationAsp.Controllers
{
    [Authorize]
    public class ExportController : Controller
    {
        private readonly IExportService _exportService;
        private readonly IUnitOfWork _uow;
        private readonly UserManager<ApplicationUser> _userManager;

        public ExportController(
            IExportService exportService,
            IUnitOfWork uow,
            UserManager<ApplicationUser> userManager)
        {
            _exportService = exportService;
            _uow = uow;
            _userManager = userManager;
        }

        public async Task<IActionResult> Csv(int applicationId)
        {
            var app = await _uow.Applications.GetByIdAsync(applicationId);
            if (app is null) return NotFound();
            if (!CanAccess(app)) return Forbid();

            var bytes = await _exportService.ExportToCsvAsync(applicationId);
            var filename = $"asvs-report-{app.Name.Replace(" ", "-")}-{DateTime.Now:yyyyMMdd}.csv";
            return File(bytes, "text/csv", filename);
        }

        private bool CanAccess(Application app)
            => User.IsInRole("Admin") || app.OwnerId == _userManager.GetUserId(User);
    }
}
