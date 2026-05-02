using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplicationAsp.Entities;
using WebApplicationAsp.Services;

namespace WebApplicationAsp.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IReportService _reportService;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(IReportService reportService, UserManager<ApplicationUser> userManager)
        {
            _reportService = reportService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User)!;
            bool isAdmin = User.IsInRole("Admin");
            var vm = await _reportService.GetDashboardViewModelAsync(userId, isAdmin);
            return View(vm);
        }
    }
}
