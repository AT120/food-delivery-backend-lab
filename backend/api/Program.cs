using BackendApi.Filters;
using BackendBl;
using BackendBl.Services;
using BackendCommon.Interfaces;
using BackendDAL;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using ProjCommon;
using ProjCommon.Filters;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.AddDB<BackendDBContext>();

builder.ConfigureToken();
builder.Services.AddJwtAuthentication(new JwtBearerEvents
{
    OnTokenValidated = AuthenticationChecker.UserIdChecker
});

builder.Services.AddSwaggerGen( c => 
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
    // c.UseInlineDefinitionsForEnums();
    // c.UseOneOfForPolymorphism();
});

builder.Services.AddScoped<IRestaurantService, RestaurantService>();
builder.Services.AddScoped<IDishService, DishService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IStaffService, StaffService>();
builder.Services.AddScoped<IOrderService, OrderService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MigrateDBWhenNecessary<BackendDBContext>();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
  