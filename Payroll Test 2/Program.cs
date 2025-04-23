using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Payroll_Test_2.Pages.Data;

var builder = WebApplication.CreateBuilder(args);

// Add database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Razor Pages
builder.Services.AddRazorPages();

// ✅ Add Authentication & Authorization
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login"; // Redirect to login if not authenticated
        options.AccessDeniedPath = "/AccessDenied"; // Redirect if user lacks permission
    });

builder.Services.AddAuthorization();

// Add session support
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

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


app.UseAuthentication();  // ✅ Ensure authentication is used
app.UseAuthorization();   // ✅ Ensure authorization is enforced
app.UseSession();         // ✅ Enable session

// Configure error handling
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.MapRazorPages();
app.Run();
