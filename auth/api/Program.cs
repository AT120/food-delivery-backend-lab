using AuthApi;
using AuthBL.Services;
using AuthCommon.Interfaces;
using AuthDAL;
using Microsoft.AspNetCore.Identity;
using ProjCommon;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.ConfigureToken();

builder.Services.AddJwtAuthentication();
builder.AddIdentityStorage();
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 5;

    options.User.RequireUniqueEmail = true;
});

builder.Services.AddAuthorization(options =>
{
    options.UpdateDefaultPolicy();
    options.AddRefreshOnlyPolicy();
});
 

builder.Services.AddScoped<IAuthService, AuthService>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MigrateAuthDBWhenNecessary();
}

// await app.UpdateRolesAndClaims();
// app.UseHttpLogging();

app.UseHttpsRedirection();
app.UseMiddleware<DebugMiddleware>();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
