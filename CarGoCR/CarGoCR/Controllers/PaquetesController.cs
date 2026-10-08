using CarGoCR.Data;
using CarGoCR.Models;
using CarGoCR.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CarGoCR.Controllers
{
    [Authorize]
    public class PaquetesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IServicioEmail _servicioEmail;

        public PaquetesController(AppDbContext context, IServicioEmail servicioEmail)
        {
            _context = context;
            _servicioEmail = servicioEmail;
        }

        // RASTREO PÚBLICO (usado por el sitio web público, sin login)
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Rastrear(string tracking)
        {
            if (string.IsNullOrWhiteSpace(tracking))
                return Json(new { encontrado = false });

            var codigo = tracking.Trim().ToLower();

            var paquete = await _context.Paquetes
                .FirstOrDefaultAsync(p => p.Tracking.ToLower() == codigo);

            if (paquete == null)
                return Json(new { encontrado = false });

            return Json(new
            {
                encontrado = true,
                tracking = paquete.Tracking,
                estado = paquete.Estado,
                descripcion = paquete.Descripcion,
                fechaRecepcion = paquete.FechaRecepcion.ToString("dd/MM/yyyy")
            });
        }

        // LISTADO
        public async Task<IActionResult> Index(
    string? termino,
    string? estado,
    DateTime? fechaDesde,
    DateTime? fechaHasta)
        {
            var query = _context.Paquetes
                .Include(p => p.Cliente)
                .AsQueryable();

            // 🔍 BÚSQUEDA
            if (!string.IsNullOrWhiteSpace(termino))
            {
                termino = termino.Trim().ToLower();

                query = query.Where(p =>
                    p.Tracking.ToLower().Contains(termino) ||
                    (p.Cliente != null &&
                     p.Cliente.NombreCompleto.ToLower().Contains(termino))
                );
            }

            // 📌 FILTRO POR ESTADO
            if (!string.IsNullOrWhiteSpace(estado))
            {
                query = query.Where(p => p.Estado == estado);
            }

            // 📅 FILTRO FECHA DESDE
            if (fechaDesde.HasValue)
            {
                query = query.Where(p =>
                    p.FechaRecepcion.Date >= fechaDesde.Value.Date);
            }

            // 📅 FILTRO FECHA HASTA
            if (fechaHasta.HasValue)
            {
                query = query.Where(p =>
                    p.FechaRecepcion.Date <= fechaHasta.Value.Date);
            }

            var paquetes = await query
                .OrderByDescending(p => p.FechaRecepcion)
                .ToListAsync();

            return View(paquetes);
        }


        [HttpGet]
        public async Task<IActionResult> Buscar(
    string? termino,
    string? estado,
    DateTime? fechaDesde,
    DateTime? fechaHasta)
        {
            var query = _context.Paquetes
                .Include(p => p.Cliente)
                .AsQueryable();

            // 🔍 BÚSQUEDA
            if (!string.IsNullOrWhiteSpace(termino))
            {
                termino = termino.Trim().ToLower();

                query = query.Where(p =>
                    p.Tracking.ToLower().Contains(termino) ||
                    (p.Cliente != null &&
                     p.Cliente.NombreCompleto.ToLower().Contains(termino))
                );
            }

            // 📌 ESTADO
            if (!string.IsNullOrWhiteSpace(estado))
            {
                query = query.Where(p => p.Estado == estado);
            }

            // 📅 FECHA DESDE
            if (fechaDesde.HasValue)
            {
                var desdeUtc = DateTime.SpecifyKind(
                    fechaDesde.Value.Date,
                    DateTimeKind.Utc
                );

                query = query.Where(p =>
                    p.FechaRecepcion >= desdeUtc);
            }

            // 📅 FECHA HASTA
            if (fechaHasta.HasValue)
            {
                var hastaUtc = DateTime.SpecifyKind(
                    fechaHasta.Value.Date.AddDays(1),
                    DateTimeKind.Utc
                );

                query = query.Where(p =>
                    p.FechaRecepcion < hastaUtc);
            }

            var paquetes = await query
                .OrderByDescending(p => p.FechaRecepcion)
                .Take(50)
                .Select(p => new
                {
                    id = p.Id,

                    tracking = p.Tracking,

                    cliente = p.Cliente != null
                        ? p.Cliente.NombreCompleto
                        : "Sin cliente",

                    cedula = p.Cliente != null
                        ? p.Cliente.Cedula
                        : "",

                    descripcion = p.Descripcion,

                    peso = p.Peso,

                    costoEnvio = p.CostoEnvio,

                    estado = p.Estado,

                    fecha = p.FechaRecepcion
                })
                .ToListAsync();

            return Json(paquetes);
        }

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

                    if (tarifa == null)
                    {
                        ModelState.AddModelError(
                            "",
                            "No existe una tarifa configurada para ese peso.");

                        return View(paquete);
                    }

                    return View(paquete);
                }

                paquete.TarifaId = tarifa.Id;
                paquete.CostoEnvio = tarifa.PrecioInternacional;

                _context.Paquetes.Add(paquete);

                await _context.SaveChangesAsync();

                var cliente = await _context.Clientes
                    .FirstOrDefaultAsync(x => x.Id == paquete.ClienteId);

                if (cliente != null && !string.IsNullOrWhiteSpace(cliente.Correo))
                {
                    var asunto = $"Nuevo paquete registrado - {paquete.Tracking}";

                    var cuerpo = $@"
<!DOCTYPE html>
<html>
<head>
<meta charset='utf-8'>
</head>

<body style='margin:0;padding:0;background:#f4f6f9;font-family:Segoe UI,Arial,sans-serif;'>

<table width='100%' cellpadding='0' cellspacing='0' style='background:#f4f6f9;padding:40px 0;'>

<tr>
<td align='center'>

<table width='700' cellpadding='0' cellspacing='0'
style='background:#ffffff;border-radius:12px;overflow:hidden;
box-shadow:0 3px 12px rgba(0,0,0,.15);'>

<tr>
<td style='background:#0F4C81;padding:30px;text-align:center;'>

<img src='https://TU-DOMINIO.com/img/Logo.jpeg'
style='height:80px;' />

<h1 style='color:white;margin-top:20px;font-size:28px;'>
CarGo CR
</h1>

<p style='color:#dbeafe;font-size:15px;margin-top:8px;'>
Nuevo paquete registrado
</p>

</td>
</tr>

<tr>

<td style='padding:40px;'>

<p style='font-size:17px;'>

Hola <strong>{cliente.NombreCompleto}</strong>,

</p>

<p>
Su paquete ha sido registrado exitosamente en CarGo CR.
</p>

<table width='100%'
style='border-collapse:collapse;margin-top:30px;'>

<tr style='background:#f8fafc;'>
<td style='padding:12px;font-weight:bold;width:180px;'>Tracking</td>
<td style='padding:12px;'>{paquete.Tracking}</td>
</tr>

<tr>
<td style='padding:12px;font-weight:bold;'>Descripción</td>
<td style='padding:12px;'>{paquete.Descripcion}</td>
</tr>

<tr style='background:#f8fafc;'>
<td style='padding:12px;font-weight:bold;'>Estado</td>
<td style='padding:12px;'>

<span style='background:#2563eb;
color:white;
padding:6px 14px;
border-radius:20px;
font-size:13px;
font-weight:bold;'>

{paquete.Estado}

</span>

</td>
</tr>

<tr>
<td style='padding:12px;font-weight:bold;'>Peso</td>
<td style='padding:12px;'>{paquete.Peso} kg</td>
</tr>

<tr style='background:#f8fafc;'>
<td style='padding:12px;font-weight:bold;'>Costo del envío</td>
<td style='padding:12px;'>₡{paquete.CostoEnvio:N2}</td>
</tr>

<tr>
<td style='padding:12px;font-weight:bold;'>Fecha de recepción</td>
<td style='padding:12px;'>{paquete.FechaRecepcion:dd/MM/yyyy}</td>
</tr>

</table>

<div style='margin-top:40px;text-align:center;'>

</div>

<hr style='margin:40px 0;border:none;border-top:1px solid #ddd;'>

<p style='font-size:13px;color:#777;text-align:center;'>

Este correo fue generado automáticamente por
<strong>CarGo CR</strong>.

<br><br>

Gracias por confiar en nosotros.

</p>

</td>

</tr>

</table>

</td>

</tr>

</table>

</body>

</html>";

                    try
                    {
                        await _servicioEmail.EnviarEmail(
                            cliente.Correo,
                            asunto,
                            cuerpo);
                    }
                    catch (Exception ex)
                    {
                        // Registrar el error
                        Console.WriteLine(ex.Message);

                        // No detener el proceso
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Clientes = new SelectList(
                _context.Clientes,
                "Id",
                "NombreCompleto");

            return View(paquete);
        }

        [HttpGet]
        public async Task<IActionResult> BuscarClientes(string termino)
        {
            if (string.IsNullOrWhiteSpace(termino))
                return Json(new List<object>());

            termino = termino.Trim();

            var clientes = await _context.Clientes
                .Where(c =>
                    c.NombreCompleto.Contains(termino) ||
                    c.Cedula.Contains(termino) ||
                    c.Telefono.Contains(termino))
                .OrderBy(c => c.NombreCompleto)
                .Take(10)
                .Select(c => new
                {
                    id = c.Id,
                    nombre = c.NombreCompleto,
                    cedula = c.Cedula,
                    telefono = c.Telefono,
                    correo = c.Correo
                })
                .ToListAsync();

            return Json(clientes);
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
                // Buscar el paquete original en la base de datos
                var paqueteOriginal = await _context.Paquetes
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (paqueteOriginal == null)
                    return NotFound();

                // Conservar el costo de envío original
                paquete.CostoEnvio = paqueteOriginal.CostoEnvio;

                _context.Update(paquete);

                await _context.SaveChangesAsync();

                var cliente = await _context.Clientes
                    .FirstOrDefaultAsync(x => x.Id == paquete.ClienteId);

                // AQUÍ CONTINÚA TU CÓDIGO DEL CORREO

                if (cliente != null && !string.IsNullOrWhiteSpace(cliente.Correo))
                {
                    var asunto = $"Actualización de su paquete - {paquete.Tracking}";

                    var cuerpo = $@"
<!DOCTYPE html>
<html>
<head>
<meta charset='utf-8'>
</head>

<body style='margin:0;padding:0;background:#f4f6f9;font-family:Segoe UI,Arial,sans-serif;'>

<table width='100%' cellpadding='0' cellspacing='0' style='background:#f4f6f9;padding:40px 0;'>

<tr>
<td align='center'>

<table width='700' cellpadding='0' cellspacing='0'
style='background:#ffffff;border-radius:12px;overflow:hidden;
box-shadow:0 3px 12px rgba(0,0,0,.15);'>

<tr>
<td style='background:#0F4C81;padding:30px;text-align:center;'>

<img src='https://TU-DOMINIO.com/img/Logo.jpeg'
style='height:80px;' />

<h1 style='color:white;margin-top:20px;font-size:28px;'>
CarGo CR
</h1>

<p style='color:#dbeafe;font-size:15px;margin-top:8px;'>
Actualización de su paquete
</p>

</td>
</tr>

<tr>

<td style='padding:40px;'>

<p style='font-size:17px;'>

Hola <strong>{cliente.NombreCompleto}</strong>,

</p>

<p>
Queremos informarle que su paquete ha sido actualizado.
</p>

<table width='100%'
style='border-collapse:collapse;margin-top:30px;'>

<tr style='background:#f8fafc;'>
<td style='padding:12px;font-weight:bold;width:180px;'>Tracking</td>
<td style='padding:12px;'>{paquete.Tracking}</td>
</tr>

<tr>
<td style='padding:12px;font-weight:bold;'>Descripción</td>
<td style='padding:12px;'>{paquete.Descripcion}</td>
</tr>

<tr style='background:#f8fafc;'>
<td style='padding:12px;font-weight:bold;'>Estado</td>
<td style='padding:12px;'>

<span style='background:#2563eb;
color:white;
padding:6px 14px;
border-radius:20px;
font-size:13px;
font-weight:bold;'>

{paquete.Estado}

</span>

</td>
</tr>

<tr>
<td style='padding:12px;font-weight:bold;'>Peso</td>
<td style='padding:12px;'>{paquete.Peso} kg</td>
</tr>

<tr style='background:#f8fafc;'>
<td style='padding:12px;font-weight:bold;'>Costo del envío</td>
<td style='padding:12px;'>₡{paquete.CostoEnvio:N2}</td>
</tr>

<tr>
<td style='padding:12px;font-weight:bold;'>Fecha de recepción</td>
<td style='padding:12px;'>{paquete.FechaRecepcion:dd/MM/yyyy}</td>
</tr>

</table>

<div style='margin-top:40px;text-align:center;'>

</div>

<hr style='margin:40px 0;border:none;border-top:1px solid #ddd;'>

<p style='font-size:13px;color:#777;text-align:center;'>

Este correo fue generado automáticamente por
<strong>CarGo CR</strong>.

<br><br>

Gracias por confiar en nosotros.

</p>

</td>

</tr>

</table>

</td>

</tr>

</table>

</body>

</html>";

                    await _servicioEmail.EnviarEmail(
                        cliente.Correo,
                        asunto,
                        cuerpo);
                }

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