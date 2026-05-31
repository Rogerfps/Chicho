using CarGoCR.Data;
using CarGoCR.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarGoCR.Controllers
{
    public class ProformasController : Controller
    {
        private readonly AppDbContext _context;

        public ProformasController(AppDbContext context)
        {
            _context = context;
        }

        // INDEX
        public async Task<IActionResult> Index()
        {
            var proformas = await _context.Proformas
                .Include(p => p.Cliente)
                .Include(p => p.Tarifa)
                .ToListAsync();

            return View(proformas);
        }

        // DETAILS
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var proforma = await _context.Proformas
                .Include(p => p.Cliente)
                .Include(p => p.Tarifa)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (proforma == null)
                return NotFound();

            return View(proforma);
        }

        // CREATE GET
        public IActionResult Create()
        {
            ViewBag.Clientes = _context.Clientes.ToList();

            ViewBag.Tarifas = _context.Tarifas
                .Where(t => t.Activo)
                .ToList();

            return View();
        }

        // CREATE POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Proforma proforma)
        {
            if (ModelState.IsValid)
            {
                if (proforma.TarifaId.HasValue)
                {
                    var tarifa = await _context.Tarifas
                        .FirstOrDefaultAsync(t => t.Id == proforma.TarifaId);

                    if (tarifa != null)
                    {
                        proforma.CostoEstimado =
                            tarifa.PrecioNacional;
                    }
                }

                _context.Add(proforma);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Clientes = _context.Clientes.ToList();

            ViewBag.Tarifas = _context.Tarifas
                .Where(t => t.Activo)
                .ToList();

            return View(proforma);
        }
    }
}

