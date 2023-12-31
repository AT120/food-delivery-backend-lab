using AuthApi;
using AuthBL;
using AuthBL.Services;
using AuthCommon.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using ProjCommon;
using ProjCommon.Configurators;
using ProjCommon.Enums;
using ProjCommon.Filters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c=>
{
    c.AddSecurityDefinition("bearerAuth", new OpenApiSecurityScheme 
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT for authoriztion" 
    });
    c.OperationFilter<AuthFilter>();
    c.SchemaFilter<EnumSchemaFilter>();
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

builder.AddUserIdentityStorage();


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
    app.MigrateAuthDB();
}
await app.SeedRoles();
await app.InitAdmin();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
