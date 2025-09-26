using DevPhone.Models;
using DevPhone.Models.DTOs;
using System.Security.Claims;

namespace DevPhone.Services
{
    public interface IJwtService
    {
        /// <summary>
        /// Genera un token JWT para el usuario especificado
        /// </summary>
        string GenerateToken(MUsuario user);

        /// <summary>
        /// Genera un refresh token
        /// </summary>
        string GenerateRefreshToken();

        /// <summary>
        /// Valida un token JWT y retorna los claims si es válido
        /// </summary>
        ClaimsPrincipal? ValidateToken(string token);

        /// <summary>
        /// Obtiene los claims de un token sin validar la expiración
        /// </summary>
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);

        /// <summary>
        /// Obtiene el ID del usuario desde los claims
        /// </summary>
        int? GetUserIdFromClaims(ClaimsPrincipal principal);

        /// <summary>
        /// Obtiene el rol del usuario desde los claims
        /// </summary>
        string? GetUserRoleFromClaims(ClaimsPrincipal principal);

        /// <summary>
        /// Verifica si un token ha expirado
        /// </summary>
        bool IsTokenExpired(string token);

        /// <summary>
        /// Obtiene la fecha de expiración de un token
        /// </summary>
        DateTime? GetTokenExpirationDate(string token);
    }
}