using CarGoCR.Data;
using CarGoCR.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

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
                .Include(x => x.Cliente)
                .ToListAsync();

            return View(proformas);
        }

        // DETAILS
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var proforma = await _context.Proformas
                .Include(x => x.Cliente)
                .Include(x => x.Detalles)
                    .ThenInclude(d => d.Paquete)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (proforma == null)
                return NotFound();

            return View(proforma);
        }

        // CREATE GET
        public IActionResult Create()
        {
            ViewBag.Clientes = _context.Clientes
                .OrderBy(x => x.NombreCompleto)
                .ToList();

            ViewBag.Paquetes = new List<Paquete>();

            return View();
        }

        // CREATE POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            int clienteId,
            List<int> paquetesSeleccionados,
            string? observaciones)
        {
            ViewBag.Clientes = _context.Clientes
                .OrderBy(x => x.NombreCompleto)
                .ToList();

            if (paquetesSeleccionados == null || !paquetesSeleccionados.Any())
            {
                ModelState.AddModelError("",
                    "Debe seleccionar al menos un paquete.");

                ViewBag.Paquetes = new List<Paquete>();

                return View();
            }

            var paquetes = await _context.Paquetes
                .Where(x =>
                    paquetesSeleccionados.Contains(x.Id) &&
                    !x.Pagado)
                .ToListAsync();

            if (!paquetes.Any())
            {
                ModelState.AddModelError("",
                    "No se encontraron paquetes válidos.");

                return View();
            }

            var proforma = new Proforma
            {
                ClienteId = clienteId,
                Observaciones = observaciones,
                Estado = "Pendiente",
                Pagada = false,
                FechaCreacion = DateTime.UtcNow,
                Total = 0
            };

            decimal total = 0;

            foreach (var paquete in paquetes)
            {
                proforma.Detalles.Add(new ProformaDetalle
                {
                    PaqueteId = paquete.Id,
                    Precio = paquete.CostoEnvio
                });

                total += paquete.CostoEnvio;
            }

            proforma.Total = total;

            _context.Proformas.Add(proforma);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // APROBAR PAGO
        public async Task<IActionResult> Aprobar(int id)
        {
            var proforma = await _context.Proformas
                .Include(x => x.Detalles)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (proforma == null)
                return NotFound();

            proforma.Pagada = true;
            proforma.Estado = "Aprobada";

            foreach (var detalle in proforma.Detalles)
            {
                var paquete = await _context.Paquetes
                    .FindAsync(detalle.PaqueteId);

                if (paquete != null)
                {
                    paquete.Pagado = true;
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details),
                new { id = proforma.Id });
        }

        // DELETE GET
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var proforma = await _context.Proformas
                .Include(x => x.Cliente)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (proforma == null)
                return NotFound();

            return View(proforma);
        }

        // DELETE POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var proforma = await _context.Proformas
                .FindAsync(id);

            if (proforma != null)
            {
                _context.Proformas.Remove(proforma);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<JsonResult> ObtenerPaquetes(int clienteId)
        {
            var paquetes = await _context.Paquetes
                .Where(x => x.ClienteId == clienteId && !x.Pagado)
                .Select(x => new
                {
                    x.Id,
                    x.Tracking,
                    x.Descripcion,
                    x.Peso,
                    x.CostoEnvio
                })
                .ToListAsync();

            return Json(paquetes);
        }

        public async Task<IActionResult> Imprimir(int id)
        {
            var proforma = await _context.Proformas
                .Include(x => x.Cliente)
                .Include(x => x.Detalles)
                    .ThenInclude(x => x.Paquete)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (proforma == null)
                return NotFound();

            var pdf = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);

                    page.Header()
                        .Text($"PROFORMA #{proforma.Id}")
                        .FontSize(20)
                        .Bold();

                    page.Content().Column(col =>
                    {
                        col.Item().Text($"Cliente: {proforma.Cliente?.NombreCompleto}");
                        col.Item().Text($"Fecha: {proforma.FechaCreacion:dd/MM/yyyy}");
                        col.Item().Text($"Estado: {proforma.Estado}");

                        col.Item().PaddingTop(20);

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(4);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(2);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Text("Tracking").Bold();
                                header.Cell().Text("Descripción").Bold();
                                header.Cell().Text("Peso").Bold();
                                header.Cell().Text("Costo").Bold();
                            });

                            foreach (var item in proforma.Detalles)
                            {
                                table.Cell().Text(item.Paquete?.Tracking);
                                table.Cell().Text(item.Paquete?.Descripcion);
                                table.Cell().Text($"{item.Paquete?.Peso} kg");
                                table.Cell().Text($"₡{item.Precio:N2}");
                            }
                        });

                        col.Item().PaddingTop(20);

                        col.Item()
                            .AlignRight()
                            .Text($"TOTAL: ₡{proforma.Total:N2}")
                            .Bold()
                            .FontSize(16);

                        if (!string.IsNullOrWhiteSpace(proforma.Observaciones))
                        {
                            col.Item().PaddingTop(15);
                            col.Item().Text($"Observaciones: {proforma.Observaciones}");
                        }
                    });
                });
            });

            var pdfBytes = pdf.GeneratePdf();

            return File(
                pdfBytes,
                "application/pdf",
                $"Proforma_{proforma.Id}.pdf");
        }
    }
}
