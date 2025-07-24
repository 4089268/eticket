using eticket.Data;
using eticket.Services;
using eticket.Validations;
using eticket.ViewModels;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<TicketsDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("TicketDB"))
);
builder.Services.AddAuthentication("NerusTicketCookieAuth")
.AddCookie("NerusTicketCookieAuth", options =>
{
    options.Cookie.Name = "NerusTicketCookie";
    options.LoginPath = "/logearse";
    options.LogoutPath = "/cerrar-sesion";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
});

// Validaciones
builder.Services.AddScoped<IValidator<ReporteRequest>, ReportValidator>();

// Servicios
builder.Services.AddScoped<ReportService>();

var app = builder.Build();

// Seed the DB
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TicketsDBContext>();
    await DbInitializer.SeedAsync(db);
}

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
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
