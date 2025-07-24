using DevPhone.Models;
using DevPhone.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DevPhone.Controllers
{
    public class OrdenServicioController : Controller
    {
        private readonly IOrdenServicioService _ordenService;
        private readonly IClienteService _clienteService;
        private readonly IUsuarioService _usuarioService;
        private readonly IDispositivoService _dispositivoService;

        public OrdenServicioController(
            IOrdenServicioService ordenService,
            IClienteService clienteService,
            IUsuarioService usuarioService,
            IDispositivoService dispositivoService)
        {
            _ordenService = ordenService;
            _clienteService = clienteService;
            _usuarioService = usuarioService;
            _dispositivoService = dispositivoService;
        }

        // GET: OrdenServicio
        public async Task<IActionResult> Index()
        {
            var lista = await _ordenService.GetAllAsync();
            return View(lista);
        }

        // GET: OrdenServicio/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var orden = await _ordenService.GetByIdAsync(id.Value);
            if (orden == null) return NotFound();
            return View(orden);
        }

        // GET: OrdenServicio/Create
        public async Task<IActionResult> Create()
        {
            ViewData["IdCliente"] = new SelectList(await _clienteService.GetAllAsync(), "IdCliente", "Nombre");
            ViewData["IdUsuario"] = new SelectList(await _usuarioService.GetAllAsync(), "IdUsuario", "NombreUsuario");
            ViewData["IdDispositivo"] = new SelectList(await _dispositivoService.GetAllAsync(), "IdDispositivo", "Serie");
            return View();
        }

        // POST: OrdenServicio/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MOrdenServicio orden)
        {
            if (!ModelState.IsValid)
            {
                ViewData["IdCliente"] = new SelectList(await _clienteService.GetAllAsync(), "IdCliente", "Nombre", orden.IdCliente);
                ViewData["IdUsuario"] = new SelectList(await _usuarioService.GetAllAsync(), "IdUsuario", "NombreUsuario", orden.IdUsuario);
                ViewData["IdDispositivo"] = new SelectList(await _dispositivoService.GetAllAsync(), "IdDispositivo", "Serie", orden.IdDispositivo);
                return View(orden);
            }

            await _ordenService.CreateAsync(orden);
            return RedirectToAction(nameof(Index));
        }

        // GET: OrdenServicio/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var orden = await _ordenService.GetByIdAsync(id.Value);
            if (orden == null) return NotFound();

            ViewData["IdCliente"] = new SelectList(await _clienteService.GetAllAsync(), "IdCliente", "Nombre", orden.IdCliente);
            ViewData["IdUsuario"] = new SelectList(await _usuarioService.GetAllAsync(), "IdUsuario", "NombreUsuario", orden.IdUsuario);
            ViewData["IdDispositivo"] = new SelectList(await _dispositivoService.GetAllAsync(), "IdDispositivo", "Serie", orden.IdDispositivo);
            return View(orden);
        }

        // POST: OrdenServicio/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MOrdenServicio orden)
        {
            if (id != orden.IdOrden) return NotFound();
            if (!ModelState.IsValid)
            {
                ViewData["IdCliente"] = new SelectList(await _clienteService.GetAllAsync(), "IdCliente", "Nombre", orden.IdCliente);
                ViewData["IdUsuario"] = new SelectList(await _usuarioService.GetAllAsync(), "IdUsuario", "NombreUsuario", orden.IdUsuario);
                ViewData["IdDispositivo"] = new SelectList(await _dispositivoService.GetAllAsync(), "IdDispositivo", "Serie", orden.IdDispositivo);
                return View(orden);
            }

            await _ordenService.UpdateAsync(orden);
            return RedirectToAction(nameof(Index));
        }

        // GET: OrdenServicio/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var orden = await _ordenService.GetByIdAsync(id.Value);
            if (orden == null) return NotFound();
            return View(orden);
        }

        // POST: OrdenServicio/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _ordenService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
