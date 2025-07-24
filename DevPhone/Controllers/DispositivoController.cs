using DevPhone.Models;
using DevPhone.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DevPhone.Controllers
{
    public class DispositivoController : Controller
    {
        private readonly IDispositivoService _dispositivoService;
        private readonly IClienteService _clienteService;

        public DispositivoController(
            IDispositivoService dispositivoService,
            IClienteService clienteService)
        {
            _dispositivoService = dispositivoService;
            _clienteService = clienteService;
        }

        // GET: Dispositivo
        public async Task<IActionResult> Index()
        {
            var lista = await _dispositivoService.GetAllAsync();
            return View(lista);
        }

        // GET: Dispositivo/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var disp = await _dispositivoService.GetByIdAsync(id.Value);
            if (disp == null) return NotFound();
            return View(disp);
        }

        // GET: Dispositivo/Create
        public async Task<IActionResult> Create()
        {
            ViewData["IdCliente"] = new SelectList(
                await _clienteService.GetAllAsync(),
                "IdCliente", "Nombre");
            return View();
        }

        // POST: Dispositivo/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MDispositivo dispositivo)
        {
            if (!ModelState.IsValid)
            {
                ViewData["IdCliente"] = new SelectList(
                    await _clienteService.GetAllAsync(),
                    "IdCliente", "Nombre", dispositivo.IdCliente);
                return View(dispositivo);
            }

            await _dispositivoService.CreateAsync(dispositivo);
            return RedirectToAction(nameof(Index));
        }

        // GET: Dispositivo/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var disp = await _dispositivoService.GetByIdAsync(id.Value);
            if (disp == null) return NotFound();

            ViewData["IdCliente"] = new SelectList(
                await _clienteService.GetAllAsync(),
                "IdCliente", "Nombre", disp.IdCliente);
            return View(disp);
        }

        // POST: Dispositivo/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MDispositivo dispositivo)
        {
            if (id != dispositivo.IdDispositivo) return NotFound();
            if (!ModelState.IsValid)
            {
                ViewData["IdCliente"] = new SelectList(
                    await _clienteService.GetAllAsync(),
                    "IdCliente", "Nombre", dispositivo.IdCliente);
                return View(dispositivo);
            }

            await _dispositivoService.UpdateAsync(dispositivo);
            return RedirectToAction(nameof(Index));
        }

        // GET: Dispositivo/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var disp = await _dispositivoService.GetByIdAsync(id.Value);
            if (disp == null) return NotFound();
            return View(disp);
        }

        // POST: Dispositivo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _dispositivoService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
