namespace DevPhone.Models.Configuration
{
    public class JwtSettings
    {
        public const string SectionName = "JwtSettings";

        public string SecretKey { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int ExpirationInMinutes { get; set; } = 60; // 1 hora por defecto
        public int RefreshTokenExpirationInDays { get; set; } = 7; // 7 días por defecto
        public bool ValidateIssuer { get; set; } = true;
        public bool ValidateAudience { get; set; } = true;
        public bool ValidateLifetime { get; set; } = true;
        public bool ValidateIssuerSigningKey { get; set; } = true;
        public int ClockSkewInMinutes { get; set; } = 5; // Tolerancia de 5 minutos
    }
}