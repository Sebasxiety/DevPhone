using DevPhone.Models;
using DevPhone.Services;
using Microsoft.AspNetCore.Mvc;

namespace DevPhone.Controllers
{
    public class RepuestoController : Controller
    {
        private readonly IRepuestoService _service;

        public RepuestoController(IRepuestoService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            var repuestos = await _service.GetAllAsync();
            return View(repuestos);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(MRepuesto repuesto)
        {
            if (ModelState.IsValid)
            {
                await _service.AddAsync(repuesto);
                return RedirectToAction(nameof(Index));
            }
            return View(repuesto);
        }
    }
}
