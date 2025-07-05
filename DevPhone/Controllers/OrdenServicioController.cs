using DevPhone.Services;
using Microsoft.AspNetCore.Mvc;

namespace DevPhone.Controllers
{
    public class OrdenServicioController : Controller
    {
        private readonly IOrdenServicioService _service;

        public OrdenServicioController(IOrdenServicioService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            var ordenes = await _service.GetAllAsync();
            return View(ordenes);
        }

        public async Task<IActionResult> Details(int id)
        {
            var orden = await _service.GetByIdAsync(id);
            if (orden == null)
            {
                return NotFound();
            }
            return View(orden);
        }
    }
}
