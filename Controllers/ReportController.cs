using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplicationAsp.Entities;
using WebApplicationAsp.Repository;
using WebApplicationAsp.Services;

namespace WebApplicationAsp.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {
        private readonly IReportService _reportService;
        private readonly IUnitOfWork _uow;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReportController(
            IReportService reportService,
            IUnitOfWork uow,
            UserManager<ApplicationUser> userManager)
        {
            _reportService = reportService;
            _uow = uow;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int applicationId)
        {
            var app = await _uow.Applications.GetByIdAsync(applicationId);
            if (app is null) return NotFound();
            if (!CanAccess(app)) return Forbid();

            var vm = await _reportService.GenerateReportAsync(applicationId);
            return View(vm);
        }

        private bool CanAccess(Application app)
            => User.IsInRole("Admin") || app.OwnerId == _userManager.GetUserId(User);
    }
}
