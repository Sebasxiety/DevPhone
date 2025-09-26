using DevPhone.Services;
using DevPhone.ViewModels;
using DevPhone.Models.DTOs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DevPhone.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IJwtService _jwtService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            IUsuarioService usuarioService, 
            IJwtService jwtService,
            ILogger<AccountController> logger)
        {
            _usuarioService = usuarioService;
            _jwtService = jwtService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            // Si el usuario ya está autenticado, redirigir
            if (User.Identity.IsAuthenticated)
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);
                
                return RedirectToAction("Index", "Home");
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginVM());
        }

        [HttpPost, Microsoft.AspNetCore.Mvc.ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM vm, string returnUrl = null)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(vm);

                var user = await _usuarioService.ValidateUserAsync(vm.Username, vm.Password);
                if (user == null)
                {
                    ModelState.AddModelError("", "Usuario o contraseña inválidos");
                    _logger.LogWarning("Intento de login fallido para usuario: {Username}", vm.Username);
                    return View(vm);
                }

                // Generar tokens JWT
                var token = _jwtService.GenerateToken(user);
                var refreshToken = _jwtService.GenerateRefreshToken();

                // Actualizar refresh token en la base de datos
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
                await _usuarioService.UpdateUserAsync(user);

                // Configurar cookies HTTP-only seguras
                SetAuthCookies(token, refreshToken);

                // Si es una petición AJAX, devolver JSON
                if (Request.Headers.XRequestedWith == "XMLHttpRequest")
                {
                    var response = new AuthResponseDto
                    {
                        Success = true,
                        Token = token,
                        RefreshToken = refreshToken,
                        ExpiresAt = _jwtService.GetTokenExpirationDate(token),
                        User = new UserInfoDto
                        {
                            Id = user.IdUsuario,
                            Username = user.NombreUsuario,
                            FullName = user.Nombres,
                            Role = user.Rol
                        },
                        Message = "Login exitoso"
                    };
                    return Json(response);
                }

                _logger.LogInformation("Usuario {UserId} ha iniciado sesión exitosamente", user.IdUsuario);

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante el proceso de login");
                ModelState.AddModelError("", "Error interno del servidor");
                return View(vm);
            }
        }

        [HttpPost, Microsoft.AspNetCore.Mvc.ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            try
            {
                // Obtener el usuario actual para limpiar el refresh token
                var userId = _jwtService.GetUserIdFromClaims(HttpContext.User);
                if (userId.HasValue)
                {
                    var user = await _usuarioService.GetUserByIdAsync(userId.Value);
                    if (user != null)
                    {
                        user.RefreshToken = null;
                        user.RefreshTokenExpiryTime = null;
                        await _usuarioService.UpdateUserAsync(user);
                    }
                }

                // Limpiar cookies
                ClearAuthCookies();

                // Limpiar autenticación de cookies (compatibilidad)
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                _logger.LogInformation("Usuario {UserId} ha cerrado sesión", userId);

                // Si es una petición AJAX, devolver JSON
                if (Request.Headers.XRequestedWith == "XMLHttpRequest")
                {
                    return Json(new { success = true, message = "Sesión cerrada exitosamente" });
                }

                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante el proceso de logout");
                return RedirectToAction("Login");
            }
        }

        [HttpPost, Microsoft.AspNetCore.Mvc.ValidateAntiForgeryToken]
        public async Task<IActionResult> RefreshToken()
        {
            try
            {
                var refreshToken = Request.Cookies["DevPhoneRefresh"];
                if (string.IsNullOrEmpty(refreshToken))
                {
                    return Json(new AuthResponseDto 
                    { 
                        Success = false, 
                        Message = "Refresh token no encontrado",
                        Errors = new List<string> { "Token de actualización requerido" }
                    });
                }

                var user = await _usuarioService.GetUserByRefreshTokenAsync(refreshToken);
                if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                {
                    return Json(new AuthResponseDto 
                    { 
                        Success = false, 
                        Message = "Refresh token inválido o expirado",
                        Errors = new List<string> { "Token de actualización inválido" }
                    });
                }

                // Generar nuevos tokens
                var newToken = _jwtService.GenerateToken(user);
                var newRefreshToken = _jwtService.GenerateRefreshToken();

                // Actualizar refresh token en la base de datos
                user.RefreshToken = newRefreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
                await _usuarioService.UpdateUserAsync(user);

                // Configurar nuevas cookies
                SetAuthCookies(newToken, newRefreshToken);

                var response = new AuthResponseDto
                {
                    Success = true,
                    Token = newToken,
                    RefreshToken = newRefreshToken,
                    ExpiresAt = _jwtService.GetTokenExpirationDate(newToken),
                    User = new UserInfoDto
                    {
                        Id = user.IdUsuario,
                        Username = user.NombreUsuario,
                        FullName = user.Nombres,
                        Role = user.Rol
                    },
                    Message = "Token actualizado exitosamente"
                };

                return Json(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar token");
                return Json(new AuthResponseDto 
                { 
                    Success = false, 
                    Message = "Error interno del servidor",
                    Errors = new List<string> { "Error al procesar la solicitud" }
                });
            }
        }

        public IActionResult AccessDenied() => View();

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return Json(new ChangePasswordResponseDto
                    {
                        Success = false,
                        Message = "Datos inválidos",
                        Errors = errors
                    });
                }

                // Obtener el ID del usuario actual
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Json(new ChangePasswordResponseDto
                    {
                        Success = false,
                        Message = "Usuario no autenticado",
                        Errors = new List<string> { "No se pudo identificar al usuario" }
                    });
                }

                // Cambiar la contraseña
                var result = await _usuarioService.ChangePasswordAsync(userId, model.CurrentPassword, model.NewPassword);

                if (result)
                {
                    return Json(new ChangePasswordResponseDto
                    {
                        Success = true,
                        Message = "Contraseña actualizada exitosamente"
                    });
                }
                else
                {
                    return Json(new ChangePasswordResponseDto
                    {
                        Success = false,
                        Message = "La contraseña actual es incorrecta",
                        Errors = new List<string> { "Verifica tu contraseña actual" }
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar contraseña para usuario {UserId}", User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                return Json(new ChangePasswordResponseDto
                {
                    Success = false,
                    Message = "Error interno del servidor",
                    Errors = new List<string> { "Error al procesar la solicitud" }
                });
            }
        }

        #region Métodos auxiliares

        private void SetAuthCookies(string token, string refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = Request.IsHttps,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddHours(1) // Token expira en 1 hora
            };

            var refreshCookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = Request.IsHttps,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7) // Refresh token expira en 7 días
            };

            Response.Cookies.Append("DevPhoneJWT", token, cookieOptions);
            Response.Cookies.Append("DevPhoneRefresh", refreshToken, refreshCookieOptions);
        }

        private void ClearAuthCookies()
        {
            var expiredCookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = Request.IsHttps,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(-1)
            };

            Response.Cookies.Append("DevPhoneJWT", "", expiredCookieOptions);
            Response.Cookies.Append("DevPhoneRefresh", "", expiredCookieOptions);
            Response.Cookies.Append("DevPhoneAuth", "", expiredCookieOptions);
        }

        #endregion
    }
}
