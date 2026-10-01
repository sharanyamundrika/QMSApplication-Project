using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using QMSApplication.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<QMSPortalDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("QMSPortalDb"),
        ServerVersion.AutoDetect(
            builder.Configuration.GetConnectionString("QMSPortalDb"))));
builder.Services.AddScoped<QMSApplication.Data.MDExcelImporter>();
// Add Razor Pages
builder.Services.AddRazorPages();
builder.Services.AddScoped<QMSApplication.Filters.MandatoryNotificationFilter>();

// Add Session
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

// Enable Session
app.UseSession();

app.UseAuthorization();

app.MapRazorPages();

app.Run();