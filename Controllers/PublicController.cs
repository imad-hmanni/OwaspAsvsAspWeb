using Microsoft.AspNetCore.Mvc;
using WebApplicationAsp.Repository;

namespace WebApplicationAsp.Controllers
{
    public class PublicController : Controller
    {
        private readonly IUnitOfWork _uow;

        public PublicController(IUnitOfWork uow) => _uow = uow;

        public async Task<IActionResult> Index()
        {
            var categories = await _uow.Categories.GetAllAsync(
                includeProperties: "SubCategories.Items");
            return View(categories.OrderBy(c => c.Code).ToList());
        }
    }
}
