using makalesistemi;
using makalesistemi.Models;
using makalesistemi.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ? MVC ve API Controller'larý ekleyelim
builder.Services.AddControllersWithViews();
builder.Services.AddControllers();

builder.Services.AddScoped<PdfService>(); // ?? PdfService'i DI Konteynerine Ekle


// ? DbContext'i DI Konteynerine ekleyelim
builder.Services.AddDbContext<Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ? Session'ý aktif hale getir
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // 30 dakika aktif kalýr
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ? CORS politikasýný ekleyelim (Geliþtirme ve Prod ayrýmý yapýldý)
builder.Services.AddCors(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        options.AddPolicy("AllowAll", policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    }
    else
    {
        options.AddPolicy("Restricted", policy =>
        {
            policy.WithOrigins("https://your-production-domain.com") // Prod ortamýnda sadece belirli domainlere izin ver
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    }
});

// ? Servisleri Dependency Injection Container'a ekle
builder.Services.AddScoped<PdfAnonymizationService>(); // Eðer PDF servisiniz varsa DI ile ekleyin

var app = builder.Build();

// ? Hata yönetimi ve güvenlik ayarlarý
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// ? CORS politikasýný burada kullan (UseRouting'ten sonra, UseAuthorization'dan önce)
app.UseCors(builder.Environment.IsDevelopment() ? "AllowAll" : "Restricted");

// ? Session middleware'ini ekle
app.UseSession();

app.UseAuthorization();

// ? Hem API hem de MVC yönlendirmeleri için
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllers(); // API Controller'larýný dahil eder

app.Run();
