using DevPhone.Models;
using DevPhone.Services;
using Microsoft.AspNetCore.Mvc;

namespace DevPhone.Controllers
{
    public class ClienteController : Controller
    {
        private readonly IClienteService _service;

        public ClienteController(IClienteService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            var clientes = await _service.GetAllAsync();
            return View(clientes);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(MCliente cliente)
        {
            if (ModelState.IsValid)
            {
                await _service.AddAsync(cliente);
                return RedirectToAction(nameof(Index));
            }
            return View(cliente);
        }
    }
}
