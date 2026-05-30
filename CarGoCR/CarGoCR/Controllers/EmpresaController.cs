using Microsoft.AspNetCore.Mvc;

namespace CarGoCR.Controllers
{
    public class EmpresaController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
