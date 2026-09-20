using IPWebsiteMVC.Data;
using IPWebsiteMVC.Models;
using IPWebsiteMVC.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()));
builder.Services.AddDbContext<MyBooksDbContext>(options =>
    options.UseMySQL(builder.Configuration.GetConnectionString("MyBooks")
        ?? throw new InvalidOperationException("Connection string 'MyBooks' fehlt.")));
builder.Services.AddScoped<KatalogService>();
builder.Services.AddScoped<IPasswordHasher<Benutzer>, PasswordHasher<Benutzer>>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "MyBooks.Auth";
        options.LoginPath = "/Konto/Anmelden";
        options.AccessDeniedPath = "/Konto/ZugriffVerweigert";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    await AdminSeeder.SicherstellenAsync(scope.ServiceProvider);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
// Liefert auch Dateien aus, die zur Laufzeit hochgeladen wurden (z.B. neue Buchcover); MapStaticAssets kennt nur Dateien vom Build.
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
