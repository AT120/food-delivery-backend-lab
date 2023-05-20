using BackendDAL;
using Microsoft.AspNetCore.Builder;
using ProjCommon;
using ProjCommon.Configurators;

namespace BackendBl;

public static class BackendDBConfigurator
{
    public static void AddBackendDB(this WebApplicationBuilder builder)
    {
        builder.AddDB<BackendDBContext>("BackendConnection");
    }

    public static void MigrateBackendDB(this WebApplication app)
        => app.MigrateDBWhenNecessary<BackendDBContext>();
}