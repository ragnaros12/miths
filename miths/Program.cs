using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using miths;
using miths.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<DataBase>(options =>
{
    options.UseLazyLoadingProxies().UseNpgsql("Host=localhost;Port=5432;Database=miths;Username=root;Password=postgres");
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme);

builder.Services.ConfigureApplicationCookie(cookie =>
{
    cookie.ExpireTimeSpan = TimeSpan.FromMinutes(5);
    cookie.LoginPath = "/Login/Login";
    cookie.LogoutPath = "/Login/Logout";
    cookie.AccessDeniedPath = "/Account/AccessDenied";
    cookie.SlidingExpiration = true;
});
builder.Services.AddMvcCore();


builder.Services.AddIdentity<User, Role>(
        options =>
        {
            options.SignIn.RequireConfirmedAccount = false;
        })
    .AddEntityFrameworkStores<DataBase>().AddRoles<Role>().AddDefaultTokenProviders();


var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();




app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}");

app.Run();