using Microsoft.AspNetCore.Mvc;

namespace CarGoCR.Controllers
{
    public class VentasController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
