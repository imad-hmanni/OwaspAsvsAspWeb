using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplicationAsp.Entities;
using WebApplicationAsp.Repository;
using WebApplicationAsp.ViewModels;

namespace WebApplicationAsp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUnitOfWork _uow;

        public AdminController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IUnitOfWork uow)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _uow = uow;
        }

        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();
            var apps = await _uow.Applications.GetAllAsync();
            var cats = await _uow.Categories.GetAllAsync();
            var items = await _uow.Items.GetAllAsync();
            var assessments = await _uow.Assessments.GetAllAsync();

            var recentUsers = new List<UserListItemViewModel>();
            foreach (var u in users.OrderByDescending(x => x.Email).Take(5))
            {
                var roles = await _userManager.GetRolesAsync(u);
                var appCount = apps.Count(a => a.OwnerId == u.Id);
                recentUsers.Add(new UserListItemViewModel
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email ?? "",
                    Role = roles.FirstOrDefault() ?? "—",
                    ApplicationCount = appCount
                });
            }

            return View(new AdminDashboardViewModel
            {
                TotalUsers = users.Count,
                TotalApplications = apps.Count(),
                TotalCategories = cats.Count(),
                TotalRequirements = items.Count(),
                TotalAssessments = assessments.Count(),
                RecentUsers = recentUsers
            });
        }

        // ── USERS ──────────────────────────────────────────────────────────────

        public async Task<IActionResult> Users()
        {
            var users = _userManager.Users.ToList();
            var apps = await _uow.Applications.GetAllAsync();
            var list = new List<UserListItemViewModel>();

            foreach (var u in users)
            {
                var roles = await _userManager.GetRolesAsync(u);
                list.Add(new UserListItemViewModel
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email ?? "",
                    Role = roles.FirstOrDefault() ?? "—",
                    ApplicationCount = apps.Count(a => a.OwnerId == u.Id)
                });
            }
            return View(list);
        }

        [HttpGet]
        public IActionResult CreateUser()
        {
            ViewBag.Roles = new SelectList(new[] { "Admin", "Auditor", "Viewer" });
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(CreateUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Roles = new SelectList(new[] { "Admin", "Auditor", "Viewer" });
                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                EmailConfirmed = true
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, model.Role);
                TempData["Success"] = $"Utilisateur {model.Email} créé avec le rôle {model.Role}.";
                return RedirectToAction(nameof(Users));
            }

            foreach (var err in result.Errors)
                ModelState.AddModelError(string.Empty, err.Description);

            ViewBag.Roles = new SelectList(new[] { "Admin", "Auditor", "Viewer" });
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user is null) return NotFound();
            var roles = await _userManager.GetRolesAsync(user);

            ViewBag.Roles = new SelectList(new[] { "Admin", "Auditor", "Viewer" });
            return View(new EditUserViewModel
            {
                Id = user.Id,
                FullName = user.FullName ?? "",
                Email = user.Email ?? "",
                Role = roles.FirstOrDefault() ?? "Viewer"
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(string id, EditUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Roles = new SelectList(new[] { "Admin", "Auditor", "Viewer" });
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user is null) return NotFound();

            user.FullName = model.FullName;
            user.Email = model.Email;
            user.UserName = model.Email;
            await _userManager.UpdateAsync(user);

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, model.Role);

            TempData["Success"] = "Utilisateur mis à jour.";
            return RedirectToAction(nameof(Users));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user is null) return NotFound();
            if (user.Email == "admin@asvs-auditor.com")
            {
                TempData["Error"] = "Impossible de supprimer l'administrateur système.";
                return RedirectToAction(nameof(Users));
            }
            await _userManager.DeleteAsync(user);
            TempData["Success"] = "Utilisateur supprimé.";
            return RedirectToAction(nameof(Users));
        }
    }
}
