using CarGoCR.Data;
using CarGoCR.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CarGoCR.Controllers
{
    public class TarifasController : Controller
    {
        private readonly AppDbContext _context;

        public TarifasController(AppDbContext context)
        {
            _context = context;
        }

        // LISTAR TARIFAS
        public IActionResult Index()
        {
            var tarifas = _context.Tarifas.ToList();

            return View(tarifas);
        }

        // MOSTRAR FORMULARIO CREATE
        public IActionResult Create()
        {
            Tarifa tarifa = new Tarifa();

            return View(tarifa);
        }

        // GUARDAR TARIFA
        [HttpPost]
        public IActionResult Create(Tarifa tarifa)
        {
            if (ModelState.IsValid)
            {
                _context.Tarifas.Add(tarifa);

                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(tarifa);
        }

        // DETALLES
        public IActionResult Details(int id)
        {
            var tarifa = _context.Tarifas.Find(id);

            if (tarifa == null)
            {
                return NotFound();
            }

            return View(tarifa);
        }

        // MOSTRAR FORMULARIO EDIT
        public IActionResult Edit(int id)
        {
            var tarifa = _context.Tarifas.Find(id);

            if (tarifa == null)
            {
                return NotFound();
            }

            return View(tarifa);
        }

        // GUARDAR CAMBIOS
        [HttpPost]
        public IActionResult Edit(Tarifa tarifa)
        {
            if (ModelState.IsValid)
            {
                _context.Tarifas.Update(tarifa);

                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(tarifa);
        }

        // MOSTRAR CONFIRMACIÓN DELETE
        public IActionResult Delete(int id)
        {
            var tarifa = _context.Tarifas.Find(id);

            if (tarifa == null)
            {
                return NotFound();
            }

            return View(tarifa);
        }

        // ELIMINAR TARIFA
        [HttpPost]
        public IActionResult Delete(Tarifa tarifa)
        {
            var tarifaDB = _context.Tarifas.Find(tarifa.Id);

            if (tarifaDB == null)
            {
                return NotFound();
            }

            _context.Tarifas.Remove(tarifaDB);

            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
