using DemoMvc.Data;
using DemoMvc.Models; // ApplicationUser
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using DinkToPdf;
using DinkToPdf.Contracts;

var builder = WebApplication.CreateBuilder(args);

// Connection string
var connectionString = builder.Configuration.GetConnectionString("AppDbContextConnection")
                       ?? "Data Source=app.db";

// Add services
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// EF Core con SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));

// Identity configuration
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.User.RequireUniqueEmail = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<AppDbContext>();

// 👉 Registro de DinkToPdf
builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

var app = builder.Build();

// --- Seed initial data (roles and admin user)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();

    try
    {
        // Aplica migraciones ANTES del seeder
        context.Database.Migrate();

        // Ejecuta seeder
        await SeedData.InitializeAsync(services);

        Console.WriteLine(" Seeder ejecutado correctamente.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($" Error seeding data: {ex.Message}");
    }
}
// ---

// Configure pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
