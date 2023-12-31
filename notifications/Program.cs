using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using NotificationApi;
using NotificationApi.Hubs;
using ProjCommon.Configurators;
using ProjCommon.Exceptions;
using ProjCommon.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddLogging();
builder.Logging.AddConsole();
builder.Services.AddSignalR();
builder.Services.AddSingleton<IUserIdProvider, DefaultIdProvider>();
builder.ConfigureNotificationQueue();


builder.Services.AddHostedService<BackgroundNotifier>();

builder.ConfigureToken();
builder.Services.AddJwtAuthentication(new JwtBearerEvents
{
    OnMessageReceived = TokenHelper.TokenFromQuery
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy => {
        policy.AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()
        .WithOrigins(
            builder.Configuration["ClientOrigin"] 
                ?? throw new InvalidConfigException("Specify client origin in appsetings!")
            );
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();
app.UseAuthorization();

app.MapControllers();
app.MapHub<NotificationHub>("/notifications");

app.Run();
