using DevPhone.Models;
using DevPhone.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<DevPhone.Models.ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<DevPhone.Services.IClienteService, DevPhone.Services.ClienteService>();
builder.Services.AddScoped<DevPhone.Services.IUsuarioService, DevPhone.Services.UsuarioService>();
builder.Services.AddScoped<DevPhone.Services.IDispositivoService, DevPhone.Services.DispositivoService>();
builder.Services.AddScoped<DevPhone.Services.IOrdenServicioService, DevPhone.Services.OrdenServicioService>();
builder.Services.AddScoped<DevPhone.Services.IRepuestoService, DevPhone.Services.RepuestoService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
