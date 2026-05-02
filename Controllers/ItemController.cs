using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplicationAsp.Entities;
using WebApplicationAsp.Repository;
using WebApplicationAsp.ViewModels;

namespace WebApplicationAsp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ItemController : Controller
    {
        private readonly IUnitOfWork _uow;
        public ItemController(IUnitOfWork uow) => _uow = uow;

        public async Task<IActionResult> Index()
        {
            var items = await _uow.Items.GetAllAsync(includeProperties: "SubCategory.Category");
            return View(items.OrderBy(i => i.Code).ToList());
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateSubCategoriesDropdown();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ItemCreateViewModel model)
        {
            if (!ModelState.IsValid) { await PopulateSubCategoriesDropdown(); return View(model); }
            await _uow.Items.AddAsync(new Item
            {
                Code = model.Code,
                Description = model.Description,
                Level = model.Level,
                SubCategoryId = model.SubCategoryId
            });
            await _uow.CompleteAsync();
            TempData["Success"] = "Exigence créée.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _uow.Items.GetByIdAsync(id);
            if (item is null) return NotFound();
            await PopulateSubCategoriesDropdown(item.SubCategoryId);
            return View(new ItemCreateViewModel
            {
                Code = item.Code,
                Description = item.Description,
                Level = item.Level,
                SubCategoryId = item.SubCategoryId
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ItemCreateViewModel model)
        {
            if (!ModelState.IsValid) { await PopulateSubCategoriesDropdown(model.SubCategoryId); return View(model); }
            var item = await _uow.Items.GetByIdAsync(id);
            if (item is null) return NotFound();
            item.Code = model.Code;
            item.Description = model.Description;
            item.Level = model.Level;
            item.SubCategoryId = model.SubCategoryId;
            _uow.Items.Update(item);
            await _uow.CompleteAsync();
            TempData["Success"] = "Exigence mise à jour.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _uow.Items.GetByIdAsync(id);
            if (item is null) return NotFound();
            _uow.Items.Remove(item);
            await _uow.CompleteAsync();
            TempData["Success"] = "Exigence supprimée.";
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateSubCategoriesDropdown(int? selected = null)
        {
            var subs = await _uow.SubCategories.GetAllAsync(includeProperties: "Category");
            ViewBag.SubCategories = new SelectList(
                subs.OrderBy(s => s.Code).Select(s => new { s.Id, Label = $"{s.Code} — {s.Name}" }),
                "Id", "Label", selected);
        }
    }
}
