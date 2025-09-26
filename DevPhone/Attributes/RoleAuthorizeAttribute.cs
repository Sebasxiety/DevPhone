using DevPhone.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace DevPhone.Attributes
{
    /// <summary>
    /// Atributo de autorización personalizado que verifica roles específicos
    /// </summary>
    public class RoleAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly UserRole[] _allowedRoles;

        public RoleAuthorizeAttribute(params UserRole[] allowedRoles)
        {
            _allowedRoles = allowedRoles ?? throw new ArgumentNullException(nameof(allowedRoles));
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Verificar si el usuario está autenticado
            if (!context.HttpContext.User.Identity?.IsAuthenticated ?? true)
            {
                HandleUnauthorized(context);
                return;
            }

            // Obtener el rol del usuario
            var userRoleClaim = context.HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;
            if (string.IsNullOrEmpty(userRoleClaim))
            {
                HandleUnauthorized(context);
                return;
            }

            // Verificar si el rol del usuario está en los roles permitidos
            var userRole = UserRoleExtensions.FromString(userRoleClaim);
            if (!_allowedRoles.Contains(userRole))
            {
                HandleForbidden(context);
                return;
            }
        }

        private static void HandleUnauthorized(AuthorizationFilterContext context)
        {
            if (IsAjaxRequest(context.HttpContext.Request))
            {
                context.Result = new JsonResult(new 
                { 
                    success = false, 
                    message = "No autorizado. Debe iniciar sesión.",
                    redirectUrl = "/Account/Login"
                })
                {
                    StatusCode = 401
                };
            }
            else
            {
                context.Result = new RedirectToActionResult("Login", "Account", new { returnUrl = context.HttpContext.Request.Path });
            }
        }

        private static void HandleForbidden(AuthorizationFilterContext context)
        {
            if (IsAjaxRequest(context.HttpContext.Request))
            {
                context.Result = new JsonResult(new 
                { 
                    success = false, 
                    message = "Acceso denegado. No tiene permisos suficientes.",
                    redirectUrl = "/Account/AccessDenied"
                })
                {
                    StatusCode = 403
                };
            }
            else
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Account", null);
            }
        }

        private static bool IsAjaxRequest(HttpRequest request)
        {
            return request.Headers.XRequestedWith == "XMLHttpRequest" ||
                   request.Headers.Accept.ToString().Contains("application/json");
        }
    }

    /// <summary>
    /// Atributo específico para autorización de administradores
    /// </summary>
    public class AdminAuthorizeAttribute : RoleAuthorizeAttribute
    {
        public AdminAuthorizeAttribute() : base(UserRole.Admin) { }
    }

    /// <summary>
    /// Atributo específico para autorización de técnicos
    /// </summary>
    public class TechnicianAuthorizeAttribute : RoleAuthorizeAttribute
    {
        public TechnicianAuthorizeAttribute() : base(UserRole.Tecnico) { }
    }

    /// <summary>
    /// Atributo que permite acceso tanto a administradores como técnicos
    /// </summary>
    public class AdminOrTechnicianAuthorizeAttribute : RoleAuthorizeAttribute
    {
        public AdminOrTechnicianAuthorizeAttribute() : base(UserRole.Admin, UserRole.Tecnico) { }
    }
}