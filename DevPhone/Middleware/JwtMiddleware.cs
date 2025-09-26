using DevPhone.Services;
using Microsoft.AspNetCore.Authorization;

namespace DevPhone.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<JwtMiddleware> _logger;

        public JwtMiddleware(RequestDelegate next, ILogger<JwtMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IJwtService jwtService)
        {
            // Verificar si el endpoint permite acceso anónimo
            var endpoint = context.GetEndpoint();
            var allowAnonymous = endpoint?.Metadata?.GetMetadata<AllowAnonymousAttribute>() != null;

            if (allowAnonymous)
            {
                await _next(context);
                return;
            }

            // Intentar obtener el token de diferentes fuentes
            var token = GetTokenFromRequest(context);

            if (!string.IsNullOrEmpty(token))
            {
                var principal = jwtService.ValidateToken(token);
                if (principal != null)
                {
                    context.User = principal;
                    _logger.LogDebug("Token JWT válido para usuario {UserId}", 
                        jwtService.GetUserIdFromClaims(principal));
                }
                else
                {
                    _logger.LogWarning("Token JWT inválido o expirado");
                    
                    // Si es una petición AJAX, devolver 401
                    if (IsAjaxRequest(context))
                    {
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsync("Token inválido o expirado");
                        return;
                    }
                    
                    // Si no es AJAX, limpiar cookies y redirigir al login
                    ClearAuthCookies(context);
                    context.Response.Redirect("/Account/Login");
                    return;
                }
            }
            else
            {
                // No hay token
                _logger.LogDebug("No se encontró token JWT en la solicitud");
                
                // Si es una petición AJAX, devolver 401
                if (IsAjaxRequest(context))
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Token requerido");
                    return;
                }
                
                // Si no es AJAX y requiere autenticación, redirigir al login
                if (!allowAnonymous)
                {
                    var returnUrl = context.Request.Path + context.Request.QueryString;
                    context.Response.Redirect($"/Account/Login?returnUrl={Uri.EscapeDataString(returnUrl)}");
                    return;
                }
            }

            await _next(context);
        }

        private static string? GetTokenFromRequest(HttpContext context)
        {
            // 1. Intentar obtener de Authorization header
            var authHeader = context.Request.Headers.Authorization.FirstOrDefault();
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                return authHeader["Bearer ".Length..].Trim();
            }

            // 2. Intentar obtener de cookie HTTP-only
            if (context.Request.Cookies.TryGetValue("DevPhoneJWT", out var cookieToken))
            {
                return cookieToken;
            }

            // 3. Intentar obtener de query parameter (para casos especiales)
            if (context.Request.Query.TryGetValue("token", out var queryToken))
            {
                return queryToken.FirstOrDefault();
            }

            return null;
        }

        private static bool IsAjaxRequest(HttpContext context)
        {
            return context.Request.Headers.XRequestedWith == "XMLHttpRequest" ||
                   context.Request.Headers.Accept.ToString().Contains("application/json") ||
                   context.Request.ContentType?.Contains("application/json") == true;
        }

        private static void ClearAuthCookies(HttpContext context)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(-1)
            };

            context.Response.Cookies.Append("DevPhoneJWT", "", cookieOptions);
            context.Response.Cookies.Append("DevPhoneRefresh", "", cookieOptions);
            context.Response.Cookies.Append("DevPhoneAuth", "", cookieOptions);
        }
    }
}