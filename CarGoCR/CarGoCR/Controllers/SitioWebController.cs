using Microsoft.AspNetCore.Mvc;

namespace CarGoCR.Controllers
{
    public class SitioWebController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
