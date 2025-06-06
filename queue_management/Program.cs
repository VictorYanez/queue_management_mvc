using Microsoft.EntityFrameworkCore;
using queue_management.Data;
using queue_management.Services;

var builder = WebApplication.CreateBuilder(args);

//Configuración de la conexión a SQL Server local (Archivo de contexto)
builder.Services.AddDbContext<ApplicationDBContext>(opciones =>
opciones.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//------------------------------------------------------------------
// Configuración para manejar las políticas de cookies
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    // Establece la política SameSite como Lax o None según el entorno
    options.MinimumSameSitePolicy = SameSiteMode.Lax;
    options.Secure = CookieSecurePolicy.SameAsRequest; // Solo aplica `Secure` en HTTPS
});

// Configura cookies antifalsificación con `Secure` solo en HTTPS
builder.Services.AddAntiforgery(options =>
{
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
});

// Add services to the container.
builder.Services.AddControllersWithViews();
// (Otra sintaxis)  opciones.UseSqlServer("name=DefaultConnection"));

// 🛡️ Configuración de servicios para Manejo de Geografía
builder.Services.AddScoped<IGeographyService, GeographyService>();


var app = builder.Build();

// Aplicación de migraciones al iniciar
using (var scope = app.Services.CreateScope())
{ 
    var applicationDBContext = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
    applicationDBContext.Database.Migrate();
}
// Configure the HTTP request pipeline de middleware.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Activa la política de cookies y autorización antes de las rutas
app.UseCookiePolicy();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
