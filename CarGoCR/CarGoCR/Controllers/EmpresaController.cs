using CarGoCR.Data;
using CarGoCR.Models;
using Microsoft.AspNetCore.Mvc;

namespace CarGoCR.Controllers
{
    public class EmpresaController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public EmpresaController(
            AppDbContext context,
            IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var empresa = _context.Empresas.FirstOrDefault();

            if (empresa == null)
            {
                empresa = new Empresa();

                _context.Empresas.Add(empresa);
                _context.SaveChanges();
            }

            return View(empresa);
        }

        [HttpPost]
        public async Task<IActionResult> Index(
            Empresa empresa,
            IFormFile? archivoLogo)
        {
            if (!ModelState.IsValid)
                return View(empresa);

            var empresaDB = _context.Empresas.FirstOrDefault();

            if (empresaDB == null)
            {
                if (archivoLogo != null)
                {
                    empresa.Logo = await GuardarLogo(archivoLogo);
                }

                _context.Empresas.Add(empresa);
            }
            else
            {
                empresaDB.NombreEmpresa = empresa.NombreEmpresa;
                empresaDB.RazonSocial = empresa.RazonSocial;
                empresaDB.CedulaJuridica = empresa.CedulaJuridica;
                empresaDB.Industria = empresa.Industria;
                empresaDB.Descripcion = empresa.Descripcion;

                empresaDB.Correo = empresa.Correo;
                empresaDB.Telefono = empresa.Telefono;
                empresaDB.SitioWeb = empresa.SitioWeb;

                empresaDB.Facebook = empresa.Facebook;
                empresaDB.Instagram = empresa.Instagram;
                empresaDB.Twitter = empresa.Twitter;

                empresaDB.MonedaPrincipal = empresa.MonedaPrincipal;
                empresaDB.MonedaSecundaria = empresa.MonedaSecundaria;
                empresaDB.TipoCambio = empresa.TipoCambio;

                empresaDB.IVA = empresa.IVA;
                empresaDB.IncluirImpuestos = empresa.IncluirImpuestos;
                empresaDB.FacturacionElectronica = empresa.FacturacionElectronica;

                if (archivoLogo != null)
                {
                    empresaDB.Logo = await GuardarLogo(archivoLogo);
                }
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Datos de la empresa guardados correctamente.";

            return RedirectToAction(nameof(Index));
        }

        private async Task<string> GuardarLogo(IFormFile archivoLogo)
        {
            string carpetaUploads = Path.Combine(
                _environment.WebRootPath,
                "uploads");

            if (!Directory.Exists(carpetaUploads))
            {
                Directory.CreateDirectory(carpetaUploads);
            }

            string nombreArchivo =
                Guid.NewGuid().ToString() +
                Path.GetExtension(archivoLogo.FileName);

            string rutaCompleta =
                Path.Combine(carpetaUploads, nombreArchivo);

            using (var stream = new FileStream(
                rutaCompleta,
                FileMode.Create))
            {
                await archivoLogo.CopyToAsync(stream);
            }

            return "/uploads/" + nombreArchivo;
        }
    }
}

