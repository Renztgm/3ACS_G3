using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Payroll_Test_2.Pages.Data;

var builder = WebApplication.CreateBuilder(args);

// Add database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Razor Pages
builder.Services.AddRazorPages()
    .AddSessionStateTempDataProvider()
    .AddNewtonsoftJson();// Enable TempData for session


// ✅ Add Authentication & Authorization
builder.Services.AddAuthentication("MyCookieAuth")
    .AddCookie("MyCookieAuth", options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.Cookie.Name = "MyAppCookie";
    });

builder.Services
    .AddControllersWithViews()
    .AddNewtonsoftJson();

builder.Services.AddAuthorization();

// Add session support
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddHttpContextAccessor();

builder.Logging.AddConsole(); // Enables logging to Output window
builder.Logging.AddDebug();   // Enables logging in Debug Output

var app = builder.Build();

// Enable Middleware
app.UseHttpsRedirection();
app.UseRouting();
app.UseStaticFiles();

app.UseStatusCodePages(async context =>
{
    if (context.HttpContext.Response.StatusCode == 404)
    {
        context.HttpContext.Response.Redirect("/Error404");
    }
});

app.UseSession();         // ✅ Enable session
app.UseAuthentication();  // ✅ Ensure authentication is used
app.UseAuthorization();   // ✅ Ensure authorization is enforced


// Configure error handling
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.MapRazorPages();
app.Run();
