using AdminBL.Services;
using AdminCommon.Interfaces;
using AdminPanel;
using AdminPanel.Filters;
using AuthBL;
using BackendBl;
using BackendBl.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using ProjCommon.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(conf => 
{
    conf.Filters.Add<ModelValidationFilter>();
});


builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
builder.AddBackendDB();
builder.AddUserIdentityStorage(cookieEnabled: true);
// builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
//     .AddCookie(opts =>
//     {
//         opts.AccessDeniedPath = "/Login";
//         opts.LoginPath = "/Login";
//     });

builder.Services.ConfigureApplicationCookie(opts =>
{
        opts.AccessDeniedPath = "/Auth";
        opts.LoginPath = "/Auth";
        opts.ExpireTimeSpan = TimeSpan.FromMinutes(5);
        // opts.Events.OnSigningIn =  AuthChecker.AdminOnlySingIn;
});
builder.Services.AddScoped<IRestaurantService, RestaurantService>();
builder.Services.AddScoped<IAdminRestaurantService, AdminRestaurantService>();
builder.Services.AddScoped<IAdminUserService, AdminUserService>();
builder.Services.AddScoped<IAdminAuthService, AdminAuthService>();

var app = builder.Build();

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
    pattern: "{controller=Restaurant}/{action=Index}/{id?}");

app.Run();
