using BackendApi.FIlters;
using BackendBl.Services;
using BackendCommon.Interfaces;
using BackendDAL;
using ProjCommon;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.AddDB<BackendDBContext>();

builder.ConfigureToken();
builder.Services.AddJwtAuthentication();

builder.Services.AddSwaggerGen( c => 
{
    c.SchemaFilter<EnumSchemaFilter>();
});

builder.Services.AddScoped<IRestaurantService, RestaurantService>();

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
  