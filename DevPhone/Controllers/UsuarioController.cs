using DevPhone.Models;
using DevPhone.Services;
using Microsoft.AspNetCore.Mvc;

namespace DevPhone.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly IUsuarioService _service;

        public UsuarioController(IUsuarioService service)
        {
            _service = service;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(MUsuario usuario)
        {
            if (ModelState.IsValid)
            {
                var user = await _service.LoginAsync(usuario.UserName, usuario.Password);
                if (user != null)
                {
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError(string.Empty, "Credenciales inválidas");
            }
            return View(usuario);
        }
    }
}
