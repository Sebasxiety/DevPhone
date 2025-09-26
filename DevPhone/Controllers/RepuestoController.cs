using DevPhone.Models;
using DevPhone.Services;
using DevPhone.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevPhone.Controllers
{
    [AdminOrTechnicianAuthorize]
    public class RepuestoController : Controller
    {
        private readonly IRepuestoService _svc;
        public RepuestoController(IRepuestoService svc) => _svc = svc;

        // GET: /Repuesto
        public async Task<IActionResult> Index()
        {
            ViewBag.Success = TempData["RepuestoSuccess"];
            ViewBag.Error = TempData["RepuestoError"];
            var lista = await _svc.GetAllAsync();
            return View(lista);
        }

        // POST: /Repuesto/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MRepuesto repuesto)
        {
            // Ignorar la navegaci�n al bindear
            ModelState.Remove(nameof(MRepuesto.DetallesRepuesto));

            if (!ModelState.IsValid)
            {
                var msgs = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);
                TempData["RepuestoError"] = string.Join(" | ", msgs);
                return RedirectToAction(nameof(Index));
            }

            await _svc.CreateAsync(repuesto);
            TempData["RepuestoSuccess"] = "Repuesto creado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // POST: /Repuesto/Edit
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MRepuesto repuesto)
        {
            ModelState.Remove(nameof(MRepuesto.DetallesRepuesto));

            if (id != repuesto.IdRepuesto || !ModelState.IsValid)
            {
                TempData["RepuestoError"] = "Error al actualizar el repuesto.";
                return RedirectToAction(nameof(Index));
            }

            await _svc.UpdateAsync(repuesto);
            TempData["RepuestoSuccess"] = "Repuesto actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // POST: /Repuesto/Delete
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _svc.DeleteAsync(id);
            TempData["RepuestoSuccess"] = "Repuesto eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}
