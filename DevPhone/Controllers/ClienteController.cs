using DevPhone.Models;
using DevPhone.Services;
using Microsoft.AspNetCore.Mvc;

namespace DevPhone.Controllers
{
    public class ClienteController : Controller
    {
        private readonly IClienteService _clienteService;

        public ClienteController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        // GET: Cliente
        public async Task<IActionResult> Index()
        {
            var lista = await _clienteService.GetAllAsync();
            return View(lista);
        }

        // GET: Cliente/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var cliente = await _clienteService.GetByIdAsync(id.Value);
            if (cliente == null) return NotFound();
            return View(cliente);
        }

        // GET: Cliente/Create
        public IActionResult Create() => View();

        // POST: Cliente/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MCliente cliente)
        {
            if (!ModelState.IsValid) return View(cliente);
            await _clienteService.CreateAsync(cliente);
            return RedirectToAction(nameof(Index));
        }

        // GET: Cliente/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var cliente = await _clienteService.GetByIdAsync(id.Value);
            if (cliente == null) return NotFound();
            return View(cliente);
        }

        // POST: Cliente/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MCliente cliente)
        {
            if (id != cliente.IdCliente) return NotFound();
            if (!ModelState.IsValid) return View(cliente);
            await _clienteService.UpdateAsync(cliente);
            return RedirectToAction(nameof(Index));
        }

        // GET: Cliente/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var cliente = await _clienteService.GetByIdAsync(id.Value);
            if (cliente == null) return NotFound();
            return View(cliente);
        }

        // POST: Cliente/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _clienteService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
