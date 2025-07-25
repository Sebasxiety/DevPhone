using DevPhone.Models;
using DevPhone.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevPhone.Controllers
{
    [Authorize]
    public class UsuarioController : Controller
    {
        private readonly IUsuarioService _svc;
        public UsuarioController(IUsuarioService svc) => _svc = svc;

        // GET: Usuario
        public async Task<IActionResult> Index()
        {
            ViewBag.Success = TempData["UsuarioSuccess"];
            ViewBag.Error = TempData["UsuarioError"];
            var lista = await _svc.GetAllAsync();
            return View(lista);
        }

        // POST: Usuario/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MUsuario usuario)
        {
            ModelState.Remove(nameof(MUsuario.Ordenes));
            usuario.FechaCreacion = DateTime.Now;

            if (!ModelState.IsValid)
            {
                var msgs = ModelState.Values
                                .SelectMany(v => v.Errors)
                                .Select(e => e.ErrorMessage);
                TempData["UsuarioError"] = string.Join(" | ", msgs);
                return RedirectToAction(nameof(Index));
            }

            await _svc.CreateAsync(usuario);
            TempData["UsuarioSuccess"] = "Usuario creado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // POST: Usuario/Edit
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MUsuario usuario)
        {
            ModelState.Remove(nameof(MUsuario.Ordenes));

            if (id != usuario.IdUsuario || !ModelState.IsValid)
            {
                TempData["UsuarioError"] = "Error al actualizar el usuario.";
                return RedirectToAction(nameof(Index));
            }

            await _svc.UpdateAsync(usuario);
            TempData["UsuarioSuccess"] = "Usuario actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // POST: Usuario/Delete
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (id == 1)
            {
                TempData["UsuarioError"] = "No se puede eliminar el usuario administrador maestro.";
                return RedirectToAction(nameof(Index));
            }
            await _svc.DeleteAsync(id);
            TempData["UsuarioSuccess"] = "Usuario eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Technicians()
        {
            var all = await _svc.GetAllAsync();
            var techs = all
                .Where(u => u.Rol.Equals("Tecnico", StringComparison.OrdinalIgnoreCase))
                .Select(u => new {
                    id = u.IdUsuario,
                    text = u.Nombres
                });
            return Json(techs);
        }
    }
}
