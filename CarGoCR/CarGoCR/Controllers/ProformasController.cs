using CarGoCR.Data;
using CarGoCR.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace CarGoCR.Controllers
{
    [Authorize]
    public class ProformasController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProformasController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
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
                .Include(x => x.Detalles)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (proforma == null)
                return RedirectToAction(nameof(Index));

            if (proforma.Detalles.Any())
            {
                _context.ProformaDetalles.RemoveRange(
                    proforma.Detalles);
            }

            _context.Proformas.Remove(proforma);

            await _context.SaveChangesAsync();

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

            var logoPath = Path.Combine(
                _env.WebRootPath,
                "img",
                "Logo.jpeg");

            var primaryColor = "#0f172a";
            var accentColor = "#2563eb";
            var lightGray = "#f8fafc";

            var pdf = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(25);

                    page.Header().Column(header =>
                    {
                        header.Item().Background(accentColor)
                            .Height(10);

                        header.Item().PaddingTop(15);

                        header.Item().Row(row =>
                        {
                            row.ConstantItem(140)
                                .Height(70)
                                .Image(logoPath);

                            row.RelativeItem()
                                .AlignRight()
                                .Column(col =>
                                {
                                    col.Item()
                                        .Text("PROFORMA")
                                        .FontSize(26)
                                        .Bold()
                                        .FontColor(primaryColor);

                                    col.Item()
                                        .Text($"# {proforma.Id}")
                                        .FontSize(18);

                                    col.Item()
                                        .Text(proforma.FechaCreacion
                                            .ToLocalTime()
                                            .ToString("dd/MM/yyyy"));
                                });
                        });
                    });

                    page.Content().PaddingVertical(15).Column(content =>
                    {
                        content.Spacing(15);

                        content.Item().Row(row =>
                        {
                            row.RelativeItem()
                                .Border(1)
                                .BorderColor("#e5e7eb")
                                .Padding(12)
                                .Column(c =>
                                {
                                    c.Item()
                                        .Text("CLIENTE")
                                        .Bold()
                                        .FontColor("#6b7280");

                                    c.Item()
                                        .Text(proforma.Cliente?.NombreCompleto ?? "")
                                        .Bold()
                                        .FontSize(13);

                                    c.Item()
                                        .Text($"ID Cliente: {proforma.ClienteId}");
                                });

                            row.ConstantItem(15);

                            row.RelativeItem()
                                .Border(1)
                                .BorderColor("#e5e7eb")
                                .Padding(12)
                                .Column(c =>
                                {
                                    c.Item()
                                        .Text("INFORMACIÓN")
                                        .Bold()
                                        .FontColor("#6b7280");

                                    c.Item()
                                        .Text($"Estado: {proforma.Estado}");

                                    c.Item()
                                        .Text($"Pagada: {(proforma.Pagada ? "Sí" : "No")}");
                                });
                        });

                        content.Item().PaddingTop(10);

                        content.Item().Table(table =>
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
                                header.Cell()
                                    .Background(accentColor)
                                    .Padding(8)
                                    .Text("Tracking")
                                    .FontColor(Colors.White)
                                    .Bold();

                                header.Cell()
                                    .Background(accentColor)
                                    .Padding(8)
                                    .Text("Descripción")
                                    .FontColor(Colors.White)
                                    .Bold();

                                header.Cell()
                                    .Background(accentColor)
                                    .Padding(8)
                                    .AlignRight()
                                    .Text("Peso")
                                    .FontColor(Colors.White)
                                    .Bold();

                                header.Cell()
                                    .Background(accentColor)
                                    .Padding(8)
                                    .AlignRight()
                                    .Text("Costo")
                                    .FontColor(Colors.White)
                                    .Bold();
                            });

                            foreach (var item in proforma.Detalles)
                            {
                                table.Cell()
                                    .BorderBottom(1)
                                    .BorderColor("#e5e7eb")
                                    .Padding(6)
                                    .Text(item.Paquete?.Tracking ?? "");

                                table.Cell()
                                    .BorderBottom(1)
                                    .BorderColor("#e5e7eb")
                                    .Padding(6)
                                    .Text(item.Paquete?.Descripcion ?? "");

                                table.Cell()
                                    .BorderBottom(1)
                                    .BorderColor("#e5e7eb")
                                    .Padding(6)
                                    .AlignRight()
                                    .Text($"{item.Paquete?.Peso:N2} kg");

                                table.Cell()
                                    .BorderBottom(1)
                                    .BorderColor("#e5e7eb")
                                    .Padding(6)
                                    .AlignRight()
                                    .Text($"₡ {item.Precio:N2}");
                            }
                        });

                        content.Item().PaddingTop(20);

                        content.Item()
                            .AlignRight()
                            .Width(250)
                            .Column(total =>
                            {
                                total.Item()
                                    .BorderTop(2)
                                    .BorderColor(accentColor);

                                total.Item()
                                    .PaddingTop(10);

                                total.Item()
                                    .Row(row =>
                                    {
                                        row.RelativeItem()
                                            .Text("TOTAL")
                                            .Bold()
                                            .FontSize(14);

                                        row.RelativeItem()
                                            .AlignRight()
                                            .Text($"₡ {proforma.Total:N2}")
                                            .Bold()
                                            .FontSize(20)
                                            .FontColor(primaryColor);
                                    });
                            });

                        if (!string.IsNullOrWhiteSpace(proforma.Observaciones))
                        {
                            content.Item().PaddingTop(15);

                            content.Item()
                                .Text("OBSERVACIONES")
                                .Bold()
                                .FontColor("#6b7280");

                            content.Item()
                                .Background(lightGray)
                                .Border(1)
                                .BorderColor("#e5e7eb")
                                .Padding(10)
                                .Text(proforma.Observaciones);
                        }
                    });

                    page.Footer()
                        .AlignCenter()
                        .Text(text =>
                        {
                            text.Span("CarGo CR - ");
                            text.Span("Documento generado automáticamente");
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
