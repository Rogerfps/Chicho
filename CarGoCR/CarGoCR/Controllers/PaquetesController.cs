using CarGoCR.Data;
using CarGoCR.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CarGoCR.Controllers
{
    public class PaquetesController : Controller
    {
        private readonly AppDbContext _context;

        public PaquetesController(AppDbContext context)
        {
            _context = context;
        }

        // LISTADO
        public async Task<IActionResult> Index()
        {
            var paquetes = await _context.Paquetes
                .Include(p => p.Cliente)
                .Include(p => p.Tarifa)
                .ToListAsync();

            return View(paquetes);
        }

        // DETAILS
        // DETAILS
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var paquete = await _context.Paquetes
                .Include(p => p.Cliente)
                .Include(p => p.Tarifa)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (paquete == null)
                return NotFound();

            return View(paquete);
        }

        // CREATE GET
        public IActionResult Create()
        {
            ViewBag.Clientes = new SelectList(
                _context.Clientes,
                "Id",
                "NombreCompleto");

            return View();
        }

        // CREATE POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Paquete paquete)
        {
            if (ModelState.IsValid)
            {
                var tarifa = await _context.Tarifas
                    .Where(t =>
                        t.Activo &&
                        paquete.Peso >= t.PesoMinimo &&
                        paquete.Peso <= t.PesoMaximo)
                    .FirstOrDefaultAsync();

                if (tarifa == null)
                {
                    ModelState.AddModelError(
                        "",
                        "No existe una tarifa configurada para ese peso.");

                    ViewBag.Clientes = new SelectList(
                        _context.Clientes,
                        "Id",
                        "NombreCompleto");

                    return View(paquete);
                }

                paquete.TarifaId = tarifa.Id;
                paquete.CostoEnvio = tarifa.PrecioInternacional;

                _context.Paquetes.Add(paquete);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Clientes = new SelectList(
                _context.Clientes,
                "Id",
                "NombreCompleto");

            return View(paquete);
        }

        // EDIT GET
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var paquete = await _context.Paquetes.FindAsync(id);

            if (paquete == null)
                return NotFound();

            ViewBag.Clientes = _context.Clientes.ToList();
            ViewBag.Tarifas = _context.Tarifas
                .Where(t => t.Activo)
                .ToList();

            return View(paquete);
        }

        // EDIT POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
    int id,
    Paquete paquete)
        {
            if (id != paquete.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                var tarifa = await _context.Tarifas
                    .FirstOrDefaultAsync(t => t.Id == paquete.TarifaId);

                if (tarifa != null)
                {
                    paquete.CostoEnvio = tarifa.PrecioNacional;
                }

                _context.Update(paquete);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Clientes = _context.Clientes.ToList();
            ViewBag.Tarifas = _context.Tarifas
                .Where(t => t.Activo)
                .ToList();

            return View(paquete);
        }

        // DELETE GET
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var paquete = await _context.Paquetes
                .Include(p => p.Cliente)
                .Include(p => p.Tarifa)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (paquete == null)
                return NotFound();

            return View(paquete);
        }

        // DELETE POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var paquete = await _context.Paquetes.FindAsync(id);

            if (paquete != null)
            {
                _context.Paquetes.Remove(paquete);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

