using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplicationAsp.Entities;
using WebApplicationAsp.Repository;
using WebApplicationAsp.Services;
using WebApplicationAsp.ViewModels;

namespace WebApplicationAsp.Controllers
{
    [Authorize]
    public class ApplicationController : Controller
    {
        private readonly IUnitOfWork _uow;
        private readonly IReportService _reportService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ApplicationController(
            IUnitOfWork uow,
            IReportService reportService,
            UserManager<ApplicationUser> userManager)
        {
            _uow = uow;
            _reportService = reportService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User)!;
            bool isAdmin = User.IsInRole("Admin");

            var apps = isAdmin
                ? await _uow.Applications.GetAllAsync()
                : await _uow.Applications.GetAllAsync(filter: a => a.OwnerId == userId);

            var categories = await _uow.Categories.GetAllAsync(includeProperties: "SubCategories.Items");
            int totalItems = categories.SelectMany(c => c.SubCategories).SelectMany(s => s.Items).Count();

            var allAssessments = await _uow.Assessments.GetAllAsync();
            var assessmentsByApp = allAssessments.GroupBy(a => a.ApplicationId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var items = apps.OrderByDescending(a => a.CreatedAt).Select(a =>
            {
                assessmentsByApp.TryGetValue(a.Id, out var ass);
                ass ??= new List<Entities.Assessment>();
                int valid = ass.Count(x => x.Status == AssessmentStatus.Valid);
                int notValid = ass.Count(x => x.Status == AssessmentStatus.NotValid);
                int applicable = valid + notValid;
                double compliance = applicable > 0 ? Math.Round((double)valid / applicable * 100, 1) : 0;
                return new ApplicationListItemViewModel
                {
                    Id = a.Id,
                    Name = a.Name,
                    Description = a.Description,
                    CreatedAt = a.CreatedAt,
                    TotalItems = totalItems,
                    AssessedItems = ass.Count(x => x.Status != AssessmentStatus.Pending),
                    CompliancePercentage = compliance,
                    RiskLevel = compliance switch { >= 80 => "Faible", >= 50 => "Moyen", _ => applicable == 0 ? "N/A" : "Élevé" }
                };
            }).ToList();

            return View(items);
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ApplicationCreateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var userId = _userManager.GetUserId(User)!;
            await _uow.Applications.AddAsync(new Entities.Application
            {
                Name = model.Name,
                Description = model.Description,
                OwnerId = userId,
                CreatedAt = DateTime.UtcNow
            });
            await _uow.CompleteAsync();

            TempData["Success"] = "Application créée avec succès.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var app = await _uow.Applications.GetByIdAsync(id);
            if (app is null) return NotFound();
            if (!CanAccess(app)) return Forbid();

            return View(new ApplicationEditViewModel { Id = app.Id, Name = app.Name, Description = app.Description });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ApplicationEditViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var app = await _uow.Applications.GetByIdAsync(id);
            if (app is null) return NotFound();
            if (!CanAccess(app)) return Forbid();

            app.Name = model.Name;
            app.Description = model.Description;
            _uow.Applications.Update(app);
            await _uow.CompleteAsync();

            TempData["Success"] = "Application mise à jour.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var app = await _uow.Applications.GetByIdAsync(id);
            if (app is null) return NotFound();
            if (!CanAccess(app)) return Forbid();

            _uow.Applications.Remove(app);
            await _uow.CompleteAsync();
            TempData["Success"] = "Application supprimée.";
            return RedirectToAction(nameof(Index));
        }

        private bool CanAccess(Entities.Application app)
            => User.IsInRole("Admin") || app.OwnerId == _userManager.GetUserId(User);
    }
}
