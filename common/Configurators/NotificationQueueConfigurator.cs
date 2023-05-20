using EasyNetQ;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjCommon.Exceptions;

namespace ProjCommon.Configurators;

public static class NotificationQueueConfigurator
{
    private static string? defaultQueueName;
    public static string DefaultQueueName
    {
        get
        {
            return defaultQueueName
                ?? throw new InvalidCastException("You should use ConfigureNotificationQueue first");
        }
        private set { }
    }


    public static void ConfigureNotificationQueue(this WebApplicationBuilder builder)
    {
        string connectionString = builder.Configuration.GetConnectionString("RabbitMQNotifications")
            ?? throw new InvalidConfigException("Connection string for rabbitmq is empty.");

        defaultQueueName = builder.Configuration["RabbitConfig:NotificationQueueName"]
            ?? throw new InvalidConfigException("Can't find RabbitConfig:NotificationQueueName section");

        builder.Services.AddSingleton<IBus>(RabbitHutch.CreateBus(connectionString));
    }
}