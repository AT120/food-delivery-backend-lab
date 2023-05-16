using AdminBL.Services;
using AdminCommon.Interfaces;
using AdminPanel.Filters;
using AuthBL;
using BackendBl;
using BackendBl.Services;
using ProjCommon.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(conf => 
{
    conf.Filters.Add<ModelValidationFilter>();
});


builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
builder.AddBackendDB();
builder.AddUserIdentityStorage();


builder.Services.AddScoped<IRestaurantService, RestaurantService>();
builder.Services.AddScoped<IAdminRestaurantService, AdminRestaurantService>();
builder.Services.AddScoped<IAdminUserService, AdminUserService>();

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
