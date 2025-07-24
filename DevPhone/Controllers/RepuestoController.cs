using DevPhone.Models;
using DevPhone.Services;
using Microsoft.AspNetCore.Mvc;

namespace DevPhone.Controllers
{
    public class RepuestoController : Controller
    {
        private readonly IRepuestoService _repuestoService;

        public RepuestoController(IRepuestoService repuestoService)
        {
            _repuestoService = repuestoService;
        }

        // GET: Repuesto
        public async Task<IActionResult> Index()
        {
            var lista = await _repuestoService.GetAllAsync();
            return View(lista);
        }

        // GET: Repuesto/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var repuesto = await _repuestoService.GetByIdAsync(id.Value);
            if (repuesto == null) return NotFound();
            return View(repuesto);
        }

        // GET: Repuesto/Create
        public IActionResult Create() => View();

        // POST: Repuesto/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MRepuesto repuesto)
        {
            if (!ModelState.IsValid) return View(repuesto);
            await _repuestoService.CreateAsync(repuesto);
            return RedirectToAction(nameof(Index));
        }

        // GET: Repuesto/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var repuesto = await _repuestoService.GetByIdAsync(id.Value);
            if (repuesto == null) return NotFound();
            return View(repuesto);
        }

        // POST: Repuesto/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MRepuesto repuesto)
        {
            if (id != repuesto.IdRepuesto) return NotFound();
            if (!ModelState.IsValid) return View(repuesto);
            await _repuestoService.UpdateAsync(repuesto);
            return RedirectToAction(nameof(Index));
        }

        // GET: Repuesto/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var repuesto = await _repuestoService.GetByIdAsync(id.Value);
            if (repuesto == null) return NotFound();
            return View(repuesto);
        }

        // POST: Repuesto/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _repuestoService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
