using CarGoCR.Data;
using CarGoCR.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarGoCR.Controllers
{
    public class VentasController : Controller
    {
        private readonly AppDbContext _context;

        public VentasController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var proformas = await _context.Proformas
                .Include(x => x.Cliente)
                .Include(x => x.Detalles)
                    .ThenInclude(d => d.Paquete)
                .ToListAsync();

            return View(proformas);
        }
    }
}