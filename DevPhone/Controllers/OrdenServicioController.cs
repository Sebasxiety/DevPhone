using DevPhone.Models;
using DevPhone.Services;
using DevPhone.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace DevPhone.Controllers
{
    [AdminOrTechnicianAuthorize]
    public class OrdenServicioController : Controller
    {
        private readonly IOrdenServicioService _svc;
        private readonly IClienteService _cs;
        private readonly IUsuarioService _us;
        private readonly IDispositivoService _ds;
        private readonly IRepuestoService _rs;
        private readonly IDetalleRepuestoService _drs;
        private readonly IEmailService _emailService;
        
        public OrdenServicioController(
            IOrdenServicioService svc,
            IClienteService cs,
            IUsuarioService us,
            IDispositivoService ds,
            IRepuestoService rs,
            IDetalleRepuestoService drs,
            IEmailService emailService)
        {
            _svc = svc; _cs = cs; _us = us; _ds = ds; _rs = rs; _drs = drs;
            _emailService = emailService;
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
                TempData["OrdenError"] = "El técnico seleccionado no existe";
                return RedirectToAction(nameof(Index));
            }

            orden.Fecha = DateTime.Now;
            
            // Calcular el precio total
            orden.CalcularPrecioTotal();

            if (!ModelState.IsValid)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" || Request.ContentType?.Contains("application/json") == true)
                {
                    return Json(new { success = false, message = "Revisa los datos de la orden" });
                }
                TempData["OrdenError"] = "Revisa los datos de la orden";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var nuevaOrden = await _svc.CreateAsync(orden);
                
                // Si el estado es "Finalizada", establecer fecha de completado
                if (orden.Estado == "Finalizada")
                {
                    nuevaOrden.FechaCompletado = DateTime.Now;
                    await _svc.UpdateAsync(nuevaOrden);
                }
                
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" || Request.ContentType?.Contains("application/json") == true)
                {
                    return Json(new { success = true, ordenId = nuevaOrden.IdOrden, message = "Orden creada correctamente" });
                }
                
                TempData["OrdenSuccess"] = "Orden creada correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" || Request.ContentType?.Contains("application/json") == true)
                {
                    return Json(new { success = false, message = "Error al guardar la orden. Verifica los datos" });
                }
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

        // GET: /OrdenServicio/GetRepuestos
        [HttpGet]
        public async Task<IActionResult> GetRepuestos()
        {
            var repuestos = await _rs.GetAllAsync();
            return Json(repuestos.Select(r => new {
                id = r.IdRepuesto,
                nombre = r.Nombre,
                precio = r.PrecioUnitario,
                stock = r.Stock
            }));
        }

        // POST: /OrdenServicio/AgregarRepuesto
        [HttpPost]
        public async Task<IActionResult> AgregarRepuesto([FromBody] AgregarRepuestoRequest request)
        {
            try
            {
                // Verificar que el repuesto existe
                var repuesto = await _rs.GetByIdAsync(request.IdRepuesto);
                if (repuesto == null)
                {
                    return Json(new { success = false, message = "El repuesto no existe" });
                }

                // Verificar stock disponible
                if (repuesto.Stock < request.Cantidad)
                {
                    return Json(new { success = false, message = "Stock insuficiente" });
                }

                // Crear el detalle del repuesto
                var detalle = new MDetalleRepuesto
                {
                    IdOrden = request.IdOrden,
                    IdRepuesto = request.IdRepuesto,
                    Cantidad = request.Cantidad
                };

                await _drs.CreateAsync(detalle);

                // Actualizar stock del repuesto
                repuesto.Stock -= request.Cantidad;
                await _rs.UpdateAsync(repuesto);

                // Recalcular el precio total de la orden
                var orden = await _svc.GetByIdAsync(request.IdOrden);
                if (orden != null)
                {
                    orden.CalcularPrecioTotal();
                    await _svc.UpdateAsync(orden);
                }

                return Json(new { 
                    success = true, 
                    message = "Repuesto agregado correctamente",
                    subtotalRepuestos = orden?.SubtotalRepuestos ?? 0,
                    subtotalSinIva = orden?.SubtotalSinIva ?? 0,
                    iva = orden?.IVA ?? 0,
                    totalConIva = orden?.TotalConIva ?? 0
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al agregar el repuesto" });
            }
        }

        // POST: /OrdenServicio/QuitarRepuesto
        [HttpPost]
        public async Task<IActionResult> QuitarRepuesto([FromBody] QuitarRepuestoRequest request)
        {
            try
            {
                var detalle = await _drs.GetByIdAsync(request.IdDetalle);
                if (detalle == null)
                {
                    return Json(new { success = false, message = "El detalle no existe" });
                }

                // Restaurar stock del repuesto
                var repuesto = await _rs.GetByIdAsync(detalle.IdRepuesto);
                if (repuesto != null)
                {
                    repuesto.Stock += detalle.Cantidad;
                    await _rs.UpdateAsync(repuesto);
                }

                // Eliminar el detalle
                await _drs.DeleteAsync(request.IdDetalle);

                // Recalcular el precio total de la orden
                var orden = await _svc.GetByIdAsync(detalle.IdOrden);
                if (orden != null)
                {
                    orden.CalcularPrecioTotal();
                    await _svc.UpdateAsync(orden);
                }

                return Json(new { 
                    success = true, 
                    message = "Repuesto quitado correctamente",
                    subtotalRepuestos = orden?.SubtotalRepuestos ?? 0,
                    subtotalSinIva = orden?.SubtotalSinIva ?? 0,
                    iva = orden?.IVA ?? 0,
                    totalConIva = orden?.TotalConIva ?? 0
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al quitar el repuesto" });
            }
        }

        // GET: /OrdenServicio/GetDetallesRepuesto/{idOrden}
        [HttpGet]
        public async Task<IActionResult> GetDetallesRepuesto(int idOrden)
        {
            try
            {
                var detalles = await _drs.GetByOrdenIdAsync(idOrden);
                var resultado = detalles.Select(d => new {
                    idDetalle = d.IdDetalle,
                    idRepuesto = d.IdRepuesto,
                    nombreRepuesto = d.Repuesto?.Nombre ?? "",
                    cantidad = d.Cantidad,
                    precioUnitario = d.Repuesto?.PrecioUnitario ?? 0,
                    subtotal = d.Cantidad * (d.Repuesto?.PrecioUnitario ?? 0)
                });

                return Json(resultado);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al obtener los detalles" });
            }
        }

        // POST: /OrdenServicio/EnviarFactura
        [HttpPost]
        public async Task<IActionResult> EnviarFactura([FromBody] EnviarFacturaRequest request)
        {
            try
            {
                // Obtener la orden con todas las relaciones
                var orden = await _svc.GetByIdAsync(request.IdOrden);
                if (orden == null)
                {
                    return Json(new { success = false, message = "La orden no existe" });
                }

                // Obtener el cliente
                var cliente = await _cs.GetByIdAsync(orden.IdCliente);
                if (cliente == null)
                {
                    return Json(new { success = false, message = "Cliente no encontrado" });
                }

                // Verificar que el cliente tenga email
                if (string.IsNullOrEmpty(cliente.Correo))
                {
                    return Json(new { success = false, message = "El cliente no tiene email registrado" });
                }

                // Obtener los detalles de repuestos
                var detalles = await _drs.GetByOrdenIdAsync(request.IdOrden);

                // Enviar la factura por email
                var emailEnviado = await _emailService.SendInvoiceEmailAsync(
                    cliente.Correo, 
                    cliente.Nombre, 
                    orden, 
                    detalles.ToList()
                );

                if (emailEnviado)
                {
                    return Json(new { success = true, message = $"Factura enviada exitosamente a {cliente.Correo}" });
                }
                else
                {
                    return Json(new { success = false, message = "Error al enviar la factura por email" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error interno al enviar la factura" });
            }
        }

        // POST: /OrdenServicio/CompletarOrden
        [HttpPost]
        public async Task<IActionResult> CompletarOrden([FromBody] CompletarOrdenRequest request)
        {
            try
            {
                var orden = await _svc.GetByIdAsync(request.IdOrden);
                if (orden == null)
                {
                    return Json(new { success = false, message = "La orden no existe" });
                }

                // Marcar la orden como finalizada
                orden.Estado = "Finalizada";
                orden.FechaCompletado = DateTime.Now;
                await _svc.UpdateAsync(orden);

                // Si se solicita enviar factura por email
                if (request.EnviarFactura)
                {
                    var cliente = await _cs.GetByIdAsync(orden.IdCliente);
                    if (cliente != null && !string.IsNullOrEmpty(cliente.Correo))
                    {
                        var detalles = await _drs.GetByOrdenIdAsync(request.IdOrden);
                        await _emailService.SendInvoiceEmailAsync(
                            cliente.Correo, 
                            cliente.Nombre, 
                            orden, 
                            detalles.ToList()
                        );
                    }
                }

                return Json(new { 
                    success = true, 
                    message = request.EnviarFactura ? 
                        "Orden finalizada y factura enviada por email" : 
                        "Orden finalizada correctamente" 
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al completar la orden" });
            }
        }
    }

    // Clases para las requests
    public class AgregarRepuestoRequest
    {
        public int IdOrden { get; set; }
        public int IdRepuesto { get; set; }
        public int Cantidad { get; set; }
    }

    public class QuitarRepuestoRequest
    {
        public int IdDetalle { get; set; }
    }

    public class EnviarFacturaRequest
    {
        public int IdOrden { get; set; }
    }

    public class CompletarOrdenRequest
    {
        public int IdOrden { get; set; }
        public bool EnviarFactura { get; set; } = false;
    }
}
