namespace DevPhone.Models.Enums
{
    public enum UserRole
    {
        Admin = 1,
        Tecnico = 2
    }

    public static class UserRoleExtensions
    {
        public static string ToDisplayString(this UserRole role)
        {
            return role switch
            {
                UserRole.Admin => "Admin",
                UserRole.Tecnico => "Técnico",
                _ => role.ToString()
            };
        }

        public static UserRole FromString(string roleString)
        {
            if (string.IsNullOrEmpty(roleString))
                throw new ArgumentException("El rol no puede ser nulo o vacío");

            // Intentar conversión por nombre
            var result = roleString?.ToLower() switch
            {
                "admin" => UserRole.Admin,
                "técnico" or "tecnico" => UserRole.Tecnico,
                _ => (UserRole?)null
            };

            if (result.HasValue)
                return result.Value;

            // Intentar conversión por valor numérico como fallback
            if (int.TryParse(roleString, out int numericValue))
            {
                if (Enum.IsDefined(typeof(UserRole), numericValue))
                {
                    return (UserRole)numericValue;
                }
            }

            throw new ArgumentException($"Rol no válido: {roleString}");
        }

        public static bool IsValidRole(string roleString)
        {
            try
            {
                FromString(roleString);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}