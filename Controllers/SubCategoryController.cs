using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplicationAsp.Entities;
using WebApplicationAsp.Repository;
using WebApplicationAsp.ViewModels;

namespace WebApplicationAsp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SubCategoryController : Controller
    {
        private readonly IUnitOfWork _uow;
        public SubCategoryController(IUnitOfWork uow) => _uow = uow;

        public async Task<IActionResult> Index()
        {
            var subs = await _uow.SubCategories.GetAllAsync(includeProperties: "Category");
            return View(subs.OrderBy(s => s.Code).ToList());
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateCategoriesDropdown();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubCategoryCreateViewModel model)
        {
            if (!ModelState.IsValid) { await PopulateCategoriesDropdown(); return View(model); }
            await _uow.SubCategories.AddAsync(new SubCategory
            {
                Code = model.Code,
                Name = model.Name,
                CategoryId = model.CategoryId
            });
            await _uow.CompleteAsync();
            TempData["Success"] = "Section créée.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var sub = await _uow.SubCategories.GetByIdAsync(id);
            if (sub is null) return NotFound();
            await PopulateCategoriesDropdown(sub.CategoryId);
            return View(new SubCategoryCreateViewModel { Code = sub.Code, Name = sub.Name, CategoryId = sub.CategoryId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SubCategoryCreateViewModel model)
        {
            if (!ModelState.IsValid) { await PopulateCategoriesDropdown(model.CategoryId); return View(model); }
            var sub = await _uow.SubCategories.GetByIdAsync(id);
            if (sub is null) return NotFound();
            sub.Code = model.Code;
            sub.Name = model.Name;
            sub.CategoryId = model.CategoryId;
            _uow.SubCategories.Update(sub);
            await _uow.CompleteAsync();
            TempData["Success"] = "Section mise à jour.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var sub = await _uow.SubCategories.GetByIdAsync(id);
            if (sub is null) return NotFound();
            _uow.SubCategories.Remove(sub);
            await _uow.CompleteAsync();
            TempData["Success"] = "Section supprimée.";
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateCategoriesDropdown(int? selected = null)
        {
            var cats = await _uow.Categories.GetAllAsync();
            ViewBag.Categories = new SelectList(cats.OrderBy(c => c.Code), "Id", "Name", selected);
        }
    }
}
