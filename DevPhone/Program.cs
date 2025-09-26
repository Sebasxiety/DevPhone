using DevPhone.Models;
using DevPhone.Models.Configuration;
using DevPhone.Services;
using DevPhone.Middleware;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1) EF Core y ApplicationDbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2) Configuración JWT
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.SectionName));
var jwtSettings = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>();

// 3) Servicios de dominio
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IDispositivoService, DispositivoService>();
builder.Services.AddScoped<IOrdenServicioService, OrdenServicioService>();
builder.Services.AddScoped<IRepuestoService, RepuestoService>();
builder.Services.AddScoped<IDetalleRepuestoService, DetalleRepuestoService>();

// 4) Servicio de usuarios y JWT
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IJwtService, JwtService>();

// 5) Autenticación híbrida (Cookies + JWT)
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = jwtSettings?.ValidateIssuerSigningKey ?? true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings?.SecretKey ?? "")),
        ValidateIssuer = jwtSettings?.ValidateIssuer ?? true,
        ValidIssuer = jwtSettings?.Issuer,
        ValidateAudience = jwtSettings?.ValidateAudience ?? true,
        ValidAudience = jwtSettings?.Audience,
        ValidateLifetime = jwtSettings?.ValidateLifetime ?? true,
        ClockSkew = TimeSpan.FromMinutes(jwtSettings?.ClockSkewInMinutes ?? 5)
    };

    // Configurar eventos para manejar cookies
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            // Intentar obtener token de cookie si no está en header
            if (string.IsNullOrEmpty(context.Token))
            {
                context.Token = context.Request.Cookies["DevPhoneJWT"];
            }
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            {
                context.Response.Headers.Add("Token-Expired", "true");
            }
            return Task.CompletedTask;
        }
    };
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.Cookie.Name = "DevPhoneAuth";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

// 6) Configurar protección CSRF
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN";
    options.Cookie.Name = "__RequestVerificationToken";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

// 7) MVC sin filtro global de autenticación (se maneja por atributos en controladores)
builder.Services.AddControllersWithViews();

// 8) Configurar CORS para desarrollo
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevPhonePolicy", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// 9) Configurar opciones de cookies globales
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => false; // No requerir consentimiento para cookies esenciales
    options.MinimumSameSitePolicy = SameSiteMode.Strict;
    options.HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.Always;
    options.Secure = CookieSecurePolicy.SameAsRequest;
});

var app = builder.Build();

// 10) Seed inicial de usuario Admin en la tabla Usuarios
await SeedAdminUser(app);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// 11) Habilitar política de cookies
app.UseCookiePolicy();

app.UseRouting();

// 12) Habilitar CORS
app.UseCors("DevPhonePolicy");

// 13) Habilitar autenticación y autorización
app.UseAuthentication();
app.UseAuthorization();

// 15) Ruta por defecto al login
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();

static async Task SeedAdminUser(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    // Si la tabla Usuarios est� vac�a, creamos el Admin
    if (!await context.Usuarios.AnyAsync())
    {
        var admin = new MUsuario
        {
            Nombres = "Administrador",
            NombreUsuario = "admin",
            Contrasena = "Admin123!",  // ajusta contrase�a
            Rol = "Admin",
            FechaCreacion = DateTime.Now
        };
        context.Usuarios.Add(admin);
        await context.SaveChangesAsync();
    }
}
