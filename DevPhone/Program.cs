using DevPhone.Models;
using DevPhone.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1) EF Core y ApplicationDbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2) Servicios de dominio
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IDispositivoService, DispositivoService>();
builder.Services.AddScoped<IOrdenServicioService, OrdenServicioService>();
builder.Services.AddScoped<IRepuestoService, RepuestoService>();
builder.Services.AddScoped<IDetalleRepuestoService, DetalleRepuestoService>();

// 3) Servicio de usuarios contra tu tabla Usuarios
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

// 4) Autenticación por cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.Cookie.Name = "DevPhoneAuth";
    });

// 5) MVC con filtro global que exige usuario autenticado
builder.Services.AddControllersWithViews(options =>
{
    var policy = new AuthorizationPolicyBuilder()
                     .RequireAuthenticatedUser()
                     .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});

var app = builder.Build();

// 6) Seed inicial de usuario Admin en la tabla Usuarios
await SeedAdminUser(app);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 7) Habilitar autenticación y autorización
app.UseAuthentication();
app.UseAuthorization();

// 8) Ruta por defecto al login
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();

static async Task SeedAdminUser(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    // Si la tabla Usuarios está vacía, creamos el Admin
    if (!await context.Usuarios.AnyAsync())
    {
        var admin = new MUsuario
        {
            Nombres = "Administrador",
            NombreUsuario = "admin",
            Contrasena = "Admin123!",  // ajusta contraseña
            Rol = "Admin",
            FechaCreacion = DateTime.Now
        };
        context.Usuarios.Add(admin);
        await context.SaveChangesAsync();
    }
}
