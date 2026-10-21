using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WeddingAppV3.Data;
using WeddingAppV3.Models;
using WeddingAppV3.Services;

var builder = WebApplication.CreateBuilder(args);

// MVC controllers and views (as in Assignment 2).
builder.Services.AddControllersWithViews();

// The Identity pages (Login, Register, Logout ...) are Razor Pages.
builder.Services.AddRazorPages();

// Registers the DbContext (as in Assignment 2).
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// ASP.NET Core Identity with support for roles (AddRoles).
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// When someone who is not logged in opens a protected PAGE, they are sent to the
// login page. For the Web API (addresses starting with /api) a redirect makes no
// sense, so we answer 401 Unauthorized / 403 Forbidden instead.
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Events.OnRedirectToLogin = context =>
    {
        if (context.Request.Path.StartsWithSegments("/api"))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        }
        else
        {
            context.Response.Redirect(context.RedirectUri);
        }
        return Task.CompletedTask;
    };

    options.Events.OnRedirectToAccessDenied = context =>
    {
        if (context.Request.Path.StartsWithSegments("/api"))
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
        }
        else
        {
            context.Response.Redirect(context.RedirectUri);
        }
        return Task.CompletedTask;
    };
});

// Claims-based authorization: the policy "GuestManager" is met by every user who
// has the claim CanManageGuests with the value "true" (whatever their role is).
// Used with [Authorize(Policy = "GuestManager")] on the guest management pages.
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("GuestManager", policy =>
        policy.RequireClaim("CanManageGuests", "true"));
});

// The service layer. AddScoped = one StatisticsService object per HTTP request.
builder.Services.AddScoped<StatisticsService>();

var app = builder.Build();

// Creates the roles (Admin, User), the test accounts and their claims when the application starts.
using (IServiceScope scope = app.Services.CreateScope())
{
    await SeedData.InitializeAsync(scope.ServiceProvider);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// First WHO the user is (authentication), then WHAT the user may do (authorization).
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
    .WithStaticAssets();

app.Run();
