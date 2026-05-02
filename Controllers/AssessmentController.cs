using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplicationAsp.Entities;
using WebApplicationAsp.Repository;
using WebApplicationAsp.Services;
using WebApplicationAsp.ViewModels;

namespace WebApplicationAsp.Controllers
{
    [Authorize(Roles = "Admin,Auditor")]
    public class AssessmentController : Controller
    {
        private readonly IAssessmentService _assessmentService;
        private readonly IAIAssistantService _aiService;
        private readonly IUnitOfWork _uow;
        private readonly UserManager<ApplicationUser> _userManager;

        public AssessmentController(
            IAssessmentService assessmentService,
            IAIAssistantService aiService,
            IUnitOfWork uow,
            UserManager<ApplicationUser> userManager)
        {
            _assessmentService = assessmentService;
            _aiService = aiService;
            _uow = uow;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Conduct(int applicationId)
        {
            var app = await _uow.Applications.GetByIdAsync(applicationId);
            if (app is null) return NotFound();
            if (!CanAccess(app)) return Forbid();

            var vm = await _assessmentService.GetConductViewModelAsync(applicationId);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(AssessmentFormModel form)
        {
            var userId = _userManager.GetUserId(User)!;
            await _assessmentService.SaveAssessmentsAsync(form.ApplicationId, form.Items, userId);
            TempData["Success"] = "Évaluation sauvegardée avec succès.";
            return RedirectToAction("Review", new { applicationId = form.ApplicationId });
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Review(int applicationId)
        {
            var app = await _uow.Applications.GetByIdAsync(applicationId);
            if (app is null) return NotFound();
            if (!CanAccess(app)) return Forbid();

            var vm = await _assessmentService.GetReviewViewModelAsync(applicationId);
            return View(vm);
        }

        [HttpGet]
        [Authorize]
        public async Task<JsonResult> AiExplain(int itemId, string technology = "")
        {
            var result = await _aiService.ExplainRequirementAsync(itemId, technology);
            return Json(result);
        }

        private bool CanAccess(Application app)
            => User.IsInRole("Admin") || app.OwnerId == _userManager.GetUserId(User);
    }
}
