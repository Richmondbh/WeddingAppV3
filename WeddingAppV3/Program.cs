using Microsoft.EntityFrameworkCore;
using WeddingAppV3.Data;
using WeddingAppV3.Models;

var builder = WebApplication.CreateBuilder(args);

// MVC controllers and views 
builder.Services.AddControllersWithViews();

// The Identity pages (Login, Register, Logout ...) 
// switches on razor pages.
builder.Services.AddRazorPages();

// Registers the DbContext 
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registers ASP.NET Core Identity and tells it to store users through my DbContext.
// RequireConfirmedAccount = false: a new user can log in directly without
// confirming the email address (I willnot send emails in this assignment).
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// finding out WHO the user is (authentication),
// then decide WHAT the user may do (authorization).
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// Maps the Identity Razor Pages e.g. /Identity/Account/Login.
app.MapRazorPages()
    .WithStaticAssets();

app.Run();
