using DevPhone.Models;
using DevPhone.Services;
using DevPhone.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DevPhone.Controllers
{
    [AdminOrTechnicianAuthorize]
    public class OrdenServicioController : Controller
    {
        private readonly IOrdenServicioService _svc;
        private readonly IClienteService _cs;
        private readonly IUsuarioService _us;
        private readonly IDispositivoService _ds;
        public OrdenServicioController(
            IOrdenServicioService svc,
            IClienteService cs,
            IUsuarioService us,
            IDispositivoService ds)
        {
            _svc = svc; _cs = cs; _us = us; _ds = ds;
        }

        // GET: /OrdenServicio
        public async Task<IActionResult> Index()
        {
            ViewBag.Success = TempData["OrdenSuccess"];
            ViewBag.Error = TempData["OrdenError"];
            var lista = await _svc.GetAllAsync();
            return View(lista);
        }

        // POST: /OrdenServicio/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MOrdenServicio orden)
        {
            // Validaciones adicionales
            ModelState.Remove(nameof(MOrdenServicio.Cliente));
            ModelState.Remove(nameof(MOrdenServicio.Usuario));
            ModelState.Remove(nameof(MOrdenServicio.Dispositivo));
            ModelState.Remove(nameof(MOrdenServicio.DetallesRepuesto));

            // Verificar que el dispositivo existe
            var dispositivoExists = await _ds.ExistsAsync(orden.IdDispositivo);
            if (!dispositivoExists)
            {
                TempData["OrdenError"] = "El dispositivo seleccionado no existe";
                return RedirectToAction(nameof(Index));
            }

            // Verificar que el cliente existe
            var clienteExists = await _cs.ExistsAsync(orden.IdCliente);
            if (!clienteExists)
            {
                TempData["OrdenError"] = "El cliente seleccionado no existe";
                return RedirectToAction(nameof(Index));
            }

            // Verificar que el t�cnico existe
            var tecnicoExists = await _us.ExistsAsync(orden.IdUsuario);
            if (!tecnicoExists)
            {
                TempData["OrdenError"] = "El t�cnico seleccionado no existe";
                return RedirectToAction(nameof(Index));
            }

            orden.Fecha = DateTime.Now;

            if (!ModelState.IsValid)
            {
                TempData["OrdenError"] = "Revisa los datos de la orden";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                await _svc.CreateAsync(orden);
                TempData["OrdenSuccess"] = "Orden creada correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                TempData["OrdenError"] = "Error al guardar la orden. Verifica los datos";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: /OrdenServicio/Edit
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MOrdenServicio orden)
        {
            ModelState.Remove(nameof(MOrdenServicio.Cliente));
            ModelState.Remove(nameof(MOrdenServicio.Usuario));
            ModelState.Remove(nameof(MOrdenServicio.Dispositivo));
            ModelState.Remove(nameof(MOrdenServicio.DetallesRepuesto));

            if (id != orden.IdOrden || !ModelState.IsValid)
            {
                TempData["OrdenError"] = "Error al actualizar la orden.";
                return RedirectToAction(nameof(Index));
            }

            await _svc.UpdateAsync(orden);
            TempData["OrdenSuccess"] = "Orden actualizada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // POST: /OrdenServicio/Delete
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _svc.DeleteAsync(id);
            TempData["OrdenSuccess"] = "Orden eliminada correctamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}
