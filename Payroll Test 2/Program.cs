using Microsoft.EntityFrameworkCore;
using Payroll_Test_2.Data;

var builder = WebApplication.CreateBuilder(args);

// ✅ Add services BEFORE calling builder.Build()
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddRazorPages();

// ✅ Add session services
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout (30 minutes)
    options.Cookie.HttpOnly = true; // Secure session cookie
    options.Cookie.IsEssential = true; // Ensure session works without consent
});

var app = builder.Build(); // ⬅️ Build AFTER registering services

// ✅ Enable session middleware (AFTER `app.Build()`)
app.UseSession(); 

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages().WithStaticAssets();

app.Run();
