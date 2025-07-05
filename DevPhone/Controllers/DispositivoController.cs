using DevPhone.Services;
using Microsoft.AspNetCore.Mvc;

namespace DevPhone.Controllers
{
    public class DispositivoController : Controller
    {
        private readonly IDispositivoService _service;

        public DispositivoController(IDispositivoService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index(int clienteId)
        {
            var dispositivos = await _service.GetByClienteAsync(clienteId);
            ViewBag.ClienteId = clienteId;
            return View(dispositivos);
        }
    }
}
