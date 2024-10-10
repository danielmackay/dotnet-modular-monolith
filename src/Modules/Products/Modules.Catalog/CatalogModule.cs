using Common.SharedKernel.Discovery;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Modules.Catalog.Common.Persistence;
using System.Reflection;

namespace Modules.Catalog;

public static class CatalogModule
{
    private static readonly Assembly _module = typeof(CatalogModule).Assembly;

    public static void AddCatalog(this IHostApplicationBuilder builder)
    {
        builder.Services.AddHttpContextAccessor();

        builder.Services.AddValidatorsFromAssembly(_module);

        builder.AddPersistence();
    }

    public static void UseCatalog(this WebApplication app)
    {
        // app.UseInfrastructureMiddleware();

        app.DiscoverEndpoints(_module);
    }
}