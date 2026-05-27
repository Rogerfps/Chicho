using CarGoCR.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarGoCR.Data;



namespace CarGoCR.Controllers
{
    public class ClientesController : Controller
    {
        private readonly AppDbContext _context;

        public ClientesController(AppDbContext context)
        {
            _context = context;
        }

        // LISTAR CLIENTES
        public IActionResult Index()
        {
            var clientes = _context.Clientes.ToList();

            return View(clientes);
        }

        // MOSTRAR FORMULARIO
        public IActionResult Create()
        {
            Cliente cliente = new Cliente();

            return View(cliente);
        }

        // GUARDAR CLIENTE
        [HttpPost]
        public IActionResult Create(Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                _context.Clientes.Add(cliente);

                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(cliente);
        }

        // MOSTRAR FORMULARIO EDITAR
        public IActionResult Edit(int id)
        {
            var cliente = _context.Clientes.Find(id);

            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        // GUARDAR CAMBIOS
        [HttpPost]
        public IActionResult Edit(Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                _context.Clientes.Update(cliente);

                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(cliente);
        }

        // VER DETALLES
        public IActionResult Details(int id)
        {
            var cliente = _context.Clientes.Find(id);

            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        // MOSTRAR CONFIRMACIÓN
        public IActionResult Delete(int id)
        {
            var cliente = _context.Clientes.Find(id);

            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        // ELIMINAR CLIENTE
        [HttpPost]
        public IActionResult Delete(Cliente cliente)
        {
            var clienteDB = _context.Clientes.Find(cliente.Id);

            if (clienteDB == null)
            {
                return NotFound();
            }

            _context.Clientes.Remove(clienteDB);

            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}

