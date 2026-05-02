using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplicationAsp.Entities;
using WebApplicationAsp.Repository;

namespace WebApplicationAsp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _uow;

        public CategoryController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        // =========================
        // INDEX (GET)
        // =========================
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var categories = await _uow.Categories.GetAllAsync(
                includeProperties: "SubCategories.Items"
            );

            return View(categories);
        }

        // =========================
        // CREATE (GET)
        // =========================
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // =========================
        // CREATE (POST)
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid)
                return View(category);

            await _uow.Categories.AddAsync(category);
            await _uow.CompleteAsync();

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // EDIT (GET)
        // =========================
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _uow.Categories.GetByIdAsync(id);
            if (category == null)
                return NotFound();

            return View(category);
        }

        // =========================
        // EDIT (POST)
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Category category)
        {
            if (id != category.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(category);

            _uow.Categories.Update(category);
            await _uow.CompleteAsync();

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // DELETE (POST)
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _uow.Categories.GetByIdAsync(id);
            if (category == null)
                return NotFound();

            _uow.Categories.Remove(category);
            await _uow.CompleteAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}