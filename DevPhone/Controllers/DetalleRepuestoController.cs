using DevPhone.Models;
using DevPhone.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DevPhone.Controllers
{
    public class DetalleRepuestoController : Controller
    {
        private readonly IDetalleRepuestoService _detalleService;
        private readonly IOrdenServicioService _ordenService;
        private readonly IRepuestoService _repuestoService;

        public DetalleRepuestoController(
            IDetalleRepuestoService detalleService,
            IOrdenServicioService ordenService,
            IRepuestoService repuestoService)
        {
            _detalleService = detalleService;
            _ordenService = ordenService;
            _repuestoService = repuestoService;
        }

        // GET: DetalleRepuesto
        public async Task<IActionResult> Index()
        {
            var lista = await _detalleService.GetAllAsync();
            return View(lista);
        }

        // GET: DetalleRepuesto/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var detalle = await _detalleService.GetByIdAsync(id.Value);
            if (detalle == null) return NotFound();
            return View(detalle);
        }

        // GET: DetalleRepuesto/Create
        public async Task<IActionResult> Create()
        {
            ViewData["IdOrden"] = new SelectList(
                await _ordenService.GetAllAsync(),
                "IdOrden", "IdOrden");
            ViewData["IdRepuesto"] = new SelectList(
                await _repuestoService.GetAllAsync(),
                "IdRepuesto", "Nombre");
            return View();
        }

        // POST: DetalleRepuesto/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MDetalleRepuesto detalle)
        {
            if (!ModelState.IsValid)
            {
                ViewData["IdOrden"] = new SelectList(
                    await _ordenService.GetAllAsync(),
                    "IdOrden", "IdOrden", detalle.IdOrden);
                ViewData["IdRepuesto"] = new SelectList(
                    await _repuestoService.GetAllAsync(),
                    "IdRepuesto", "Nombre", detalle.IdRepuesto);
                return View(detalle);
            }

            await _detalleService.CreateAsync(detalle);
            return RedirectToAction(nameof(Index));
        }

        // GET: DetalleRepuesto/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var detalle = await _detalleService.GetByIdAsync(id.Value);
            if (detalle == null) return NotFound();

            ViewData["IdOrden"] = new SelectList(
                await _ordenService.GetAllAsync(),
                "IdOrden", "IdOrden", detalle.IdOrden);
            ViewData["IdRepuesto"] = new SelectList(
                await _repuestoService.GetAllAsync(),
                "IdRepuesto", "Nombre", detalle.IdRepuesto);
            return View(detalle);
        }

        // POST: DetalleRepuesto/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MDetalleRepuesto detalle)
        {
            if (id != detalle.IdDetalle) return NotFound();
            if (!ModelState.IsValid)
            {
                ViewData["IdOrden"] = new SelectList(
                    await _ordenService.GetAllAsync(),
                    "IdOrden", "IdOrden", detalle.IdOrden);
                ViewData["IdRepuesto"] = new SelectList(
                    await _repuestoService.GetAllAsync(),
                    "IdRepuesto", "Nombre", detalle.IdRepuesto);
                return View(detalle);
            }

            await _detalleService.UpdateAsync(detalle);
            return RedirectToAction(nameof(Index));
        }

        // GET: DetalleRepuesto/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var detalle = await _detalleService.GetByIdAsync(id.Value);
            if (detalle == null) return NotFound();
            return View(detalle);
        }

        // POST: DetalleRepuesto/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _detalleService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
