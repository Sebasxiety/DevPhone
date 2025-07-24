using DevPhone.Models;
using DevPhone.Services;
using Microsoft.AspNetCore.Mvc;

namespace DevPhone.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly IUsuarioService _usuarioService;

        public UsuarioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        // GET: Usuario
        public async Task<IActionResult> Index()
        {
            var lista = await _usuarioService.GetAllAsync();
            return View(lista);
        }

        // GET: Usuario/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var usuario = await _usuarioService.GetByIdAsync(id.Value);
            if (usuario == null) return NotFound();
            return View(usuario);
        }

        // GET: Usuario/Create
        public IActionResult Create() => View();

        // POST: Usuario/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MUsuario usuario)
        {
            if (!ModelState.IsValid) return View(usuario);
            await _usuarioService.CreateAsync(usuario);
            return RedirectToAction(nameof(Index));
        }

        // GET: Usuario/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var usuario = await _usuarioService.GetByIdAsync(id.Value);
            if (usuario == null) return NotFound();
            return View(usuario);
        }

        // POST: Usuario/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MUsuario usuario)
        {
            if (id != usuario.IdUsuario) return NotFound();
            if (!ModelState.IsValid) return View(usuario);
            await _usuarioService.UpdateAsync(usuario);
            return RedirectToAction(nameof(Index));
        }

        // GET: Usuario/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var usuario = await _usuarioService.GetByIdAsync(id.Value);
            if (usuario == null) return NotFound();
            return View(usuario);
        }

        // POST: Usuario/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _usuarioService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
