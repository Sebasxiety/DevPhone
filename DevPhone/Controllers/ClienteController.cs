using DevPhone.Models;
using DevPhone.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevPhone.Controllers
{
    [Authorize]
    public class ClienteController : Controller
    {
        private readonly IClienteService _svc;
        public ClienteController(IClienteService svc) => _svc = svc;

        // GET: /Cliente
        public async Task<IActionResult> Index()
        {
            ViewBag.Success = TempData["ClienteSuccess"];
            ViewBag.Error = TempData["ClienteError"];
            var lista = await _svc.GetAllAsync();
            return View(lista);
        }

        // POST: /Cliente/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Nombre,Apellido,Cedula,Telefono,Correo,Direccion")] MCliente cliente)
        {
            // Seteamos la fecha aquí
            cliente.FechaCreacion = DateTime.Now;

            if (!ModelState.IsValid)
            {
                var mensajes = ModelState.Values
                                    .SelectMany(v => v.Errors)
                                    .Select(e => e.ErrorMessage);
                TempData["ClienteError"] = string.Join(" | ", mensajes);
                return RedirectToAction(nameof(Index));
            }

            await _svc.CreateAsync(cliente);
            TempData["ClienteSuccess"] = "Cliente creado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // POST: /Cliente/Edit
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("IdCliente,Nombre,Apellido,Cedula,Telefono,Correo,Direccion")] MCliente cliente)
        {
            if (id != cliente.IdCliente)
            {
                TempData["ClienteError"] = "Identificador inválido.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                var mensajes = ModelState.Values
                                    .SelectMany(v => v.Errors)
                                    .Select(e => e.ErrorMessage);
                TempData["ClienteError"] = string.Join(" | ", mensajes);
                return RedirectToAction(nameof(Index));
            }

            // Opcional: conservar fecha creación original si la trajeras
            await _svc.UpdateAsync(cliente);
            TempData["ClienteSuccess"] = "Cliente actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // POST: /Cliente/Delete
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _svc.DeleteAsync(id);
            TempData["ClienteSuccess"] = "Cliente eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Search(string q)
        {
            var todos = await _svc.GetAllAsync();
            var filtrados = todos
                .Where(c =>
                    (!string.IsNullOrEmpty(c.Nombre) && c.Nombre.Contains(q, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.Apellido) && c.Apellido.Contains(q, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.Cedula) && c.Cedula.Contains(q, StringComparison.OrdinalIgnoreCase)))
                .Select(c => new {
                    id = c.IdCliente,
                    text = $"{c.Nombre} {c.Apellido} ({c.Cedula})"
                });
            return Json(filtrados);
        }

    }
}
