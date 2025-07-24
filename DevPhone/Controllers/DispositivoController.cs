using DevPhone.Models;
using DevPhone.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DevPhone.Controllers
{
    [Authorize]
    public class DispositivoController : Controller
    {
        private readonly IDispositivoService _dispositivoService;
        private readonly IClienteService _clienteService;

        public DispositivoController(IDispositivoService ds, IClienteService cs)
        {
            _dispositivoService = ds;
            _clienteService = cs;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Success = TempData["DispositivoSuccess"];
            ViewBag.Error = TempData["DispositivoError"];
            var lista = await _dispositivoService.GetAllAsync();
            return View(lista);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MDispositivo disp)
        {
            // omitimos navegación si existe
            ModelState.Remove(nameof(MDispositivo.Ordenes));
            ModelState.Remove(nameof(MDispositivo.Cliente));

            disp.FechaIngreso = DateTime.Now;
            if (!ModelState.IsValid)
            {
                TempData["DispositivoError"] = "Revisa los datos.";
                return RedirectToAction(nameof(Index));
            }

            await _dispositivoService.CreateAsync(disp);
            TempData["DispositivoSuccess"] = "Dispositivo agregado.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MDispositivo disp)
        {
            ModelState.Remove(nameof(MDispositivo.Ordenes));
            ModelState.Remove(nameof(MDispositivo.Cliente));

            if (id != disp.IdDispositivo || !ModelState.IsValid)
            {
                TempData["DispositivoError"] = "No se pudo actualizar.";
                return RedirectToAction(nameof(Index));
            }

            await _dispositivoService.UpdateAsync(disp);
            TempData["DispositivoSuccess"] = "Dispositivo actualizado.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _dispositivoService.DeleteAsync(id);
            TempData["DispositivoSuccess"] = "Dispositivo eliminado.";
            return RedirectToAction(nameof(Index));
        }
    }
}
