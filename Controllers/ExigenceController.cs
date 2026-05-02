using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApplicationAsp.Controllers
{
    public class ExigenceController : Controller
    {
        // GET: ExigenceController
        public ActionResult Index()
        {
            return View();
        }

        // GET: ExigenceController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ExigenceController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ExigenceController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ExigenceController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ExigenceController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ExigenceController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ExigenceController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
