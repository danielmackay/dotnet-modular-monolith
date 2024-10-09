using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Modules.Catalog.Categories;
using Modules.Catalog.Common.Persistence;
using Modules.Catalog.Products.Integrations;
using Modules.Catalog.Products.UseCases;

namespace Modules.Catalog;

public static class CatalogModule
{
    public static void AddCatalog(this IHostApplicationBuilder builder)
    {
        var applicationAssembly = typeof(CatalogModule).Assembly;

        builder.Services.AddHttpContextAccessor();

        builder.Services.AddValidatorsFromAssembly(applicationAssembly);

        builder.AddPersistence();
    }

    public static void UseCatalog(this WebApplication app)
    {
        // app.UseInfrastructureMiddleware();

        // // TODO: Consider source generation or reflection for endpoint mapping
        CreateCategoryCommand.Endpoint.MapEndpoint(app);
        AddProductCategoryCommand.Endpoint.MapEndpoint(app);
        RemoveProductCategoryCommand.Endpoint.MapEndpoint(app);
        GetProductQuery.Endpoint.MapEndpoint(app);
        UpdateProductPriceCommand.Endpoint.MapEndpoint(app);
        SearchProductsQuery.Endpoint.MapEndpoint(app);
    }
}
