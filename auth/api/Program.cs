using AuthApi;
using AuthBL;
using AuthBL.Services;
using AuthCommon.Interfaces;
using AuthDAL;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using ProjCommon;
using ProjCommon.Enums;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c=>
{
    var filePath = Path.Combine(System.AppContext.BaseDirectory, "AuthApi.xml");
    c.IncludeXmlComments(filePath);
});
builder.ConfigureToken();

builder.Services.AddJwtAuthentication(
    new JwtBearerEvents 
    {
        OnTokenValidated = AuthenticationChecker.RefreshChecker,
    }
);

builder.AddIdentityStorage();
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 5;

    options.User.RequireUniqueEmail = true;
    options.ClaimsIdentity.UserIdClaimType = ClaimType.UserId;
});

builder.Services.AddAuthorization(options =>
{
    options.InvokeHandlersAfterFailure = false;
    options.UpdateDefaultPolicy();
    options.AddRefreshOnlyPolicy();
});
 
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProfileService, ProfileService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MigrateDBWhenNecessary<AuthDBContext>();
}

// await app.UpdateRolesAndClaims();
// app.UseHttpLogging();

app.UseHttpsRedirection();
app.UseMiddleware<DebugMiddleware>();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
