using CarGoCR.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CarGoCR.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(
            SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(
    string userName,
    string password)
        {
            var result = await _signInManager.PasswordSignInAsync(
                userName,
                password,
                false,
                false);

            if (result.Succeeded)
            {
                return RedirectToAction(
                    "Index",
                    "Home");
            }

            ModelState.AddModelError("", "Usuario o contraseña incorrectos");
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction(
                "Login",
                "Account");
        }
    }

}
