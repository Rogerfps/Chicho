using CarGoCR.Data;
using CarGoCR.Models;
using CarGoCR.Servicios;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;
using Microsoft.AspNetCore.DataProtection;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("ConexionPostgreSQL")
    ));

// Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Persistir las claves de seguridad de ASP.NET Core
var carpetaClaves = @"C:\CarGoCR-Keys";

Directory.CreateDirectory(carpetaClaves);

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(carpetaClaves))
    .SetApplicationName("CarGoCR");

QuestPDF.Settings.License = LicenseType.Community;

builder.Services.AddTransient<IServicioEmail, ServicioEmail>();

var app = builder.Build();


// ======================================================
// CREAR ROLES Y USUARIO ADMINISTRADOR INICIAL
// ======================================================

using (var scope = app.Services.CreateScope())
{
    var roleManager =
        scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    var userManager =
        scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    // --------------------------------------------------
    // ROLES
    // --------------------------------------------------

    string[] roles =
    {
        "Administrador",
        "Operador",
        "Mensajero"
    };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(
                new IdentityRole(role));
        }
    }


    // --------------------------------------------------
    // USUARIO ADMINISTRADOR
    // --------------------------------------------------

    string usuarioAdmin = "admin";
    string passwordAdmin = "Admin123*";
    string emailAdmin = "admin@cargocr.com";

    var admin = await userManager.FindByNameAsync(usuarioAdmin);

    if (admin == null)
    {
        admin = new ApplicationUser
        {
            UserName = usuarioAdmin,
            Email = emailAdmin,
            NombreCompleto = "Administrador",
            Activo = true,
            EmailConfirmed = true
        };

        var resultado = await userManager.CreateAsync(
            admin,
            passwordAdmin);

        if (!resultado.Succeeded)
        {
            var errores = string.Join(
                ", ",
                resultado.Errors.Select(e => e.Description));

            throw new Exception(
                $"No se pudo crear el usuario administrador: {errores}");
        }
    }


    // --------------------------------------------------
    // ASIGNAR ROL ADMINISTRADOR
    // --------------------------------------------------

    if (!await userManager.IsInRoleAsync(
        admin,
        "Administrador"))
    {
        await userManager.AddToRoleAsync(
            admin,
            "Administrador");
    }
}



// ======================================================
// PIPELINE
// ======================================================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// Servir archivos de wwwroot, incluidos los logos y uploads
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=SitioWeb}/{action=Index}/{id?}");

app.Run();
