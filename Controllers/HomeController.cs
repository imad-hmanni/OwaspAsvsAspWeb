using Microsoft.AspNetCore.Mvc;

namespace WebApplicationAsp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => View();
    }
}
