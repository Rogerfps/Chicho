using CarGoCR.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CarGoCR.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }


        // LISTADO
        public IActionResult Index()
        {
            var usuarios = _userManager.Users.ToList();
            return View(usuarios);
        }

        // CREATE GET
        public IActionResult Create()
        {
            return View();
        }

        // CREATE POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            ApplicationUser usuario,
            string password)
        {
            // Validaciones manuales
            if (string.IsNullOrWhiteSpace(usuario.NombreCompleto))
            {
                ModelState.AddModelError("NombreCompleto",
                    "El nombre completo es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("",
                    "La contraseña es obligatoria.");
            }

            if (!ModelState.IsValid)
            {
                return View(usuario);
            }

            var result = await _userManager.CreateAsync(
                usuario,
                password);

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }

            // Mostrar errores de Identity
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(usuario);
        }

        // DETAILS
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var usuario = await _userManager.FindByIdAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }


        // EDIT GET
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var usuario = await _userManager.FindByIdAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // EDIT POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            string id,
            ApplicationUser model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            var usuario = await _userManager.FindByIdAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            usuario.NombreCompleto = model.NombreCompleto;
            usuario.UserName = model.UserName;
            usuario.Email = model.Email;
            usuario.PhoneNumber = model.PhoneNumber;
            usuario.Activo = model.Activo;

            var result = await _userManager.UpdateAsync(usuario);

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }
    }
}