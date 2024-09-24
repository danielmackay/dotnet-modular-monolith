namespace WebApi.Extensions;

public static class OpenApiExt
{
    public static void AddSwagger(this IServiceCollection services)
    {
        services.AddMicrosoftOpenApi();
    }

    private static void AddMicrosoftOpenApi(this IServiceCollection services)
    {
        services.AddOpenApi(options =>
        {
            // Schema transformer to set the format of decimal to 'decimal'
            options.AddSchemaTransformer((schema, context, cancellationToken) =>
            {
                // schema.Reference = new OpenApiReference();
                // schema.Reference.Id = Guid.NewGuid().ToString();
                // schema.Title = Guid.NewGuid().ToString();
                // schema.Reference = new OpenApiReference();
                // schema.Reference.Type = ReferenceType.;
                return Task.CompletedTask;
            });
        });
    }

    private static void AddNswagOpenApi(this IServiceCollection services)
    {
        // services.AddEndpointsApiExplorer();
        // services.AddSwaggerGen(setup =>
        // {
        //     setup.SwaggerDoc("v1", new OpenApiInfo
        //     {
        //         Title = "Modular Monolith API",
        //         Version = "v1",
        //     });
        //
        //     // Needed to support nested types in the schema
        //     setup.CustomSchemaIds(x => x.FullName?.Replace("+", ".", StringComparison.Ordinal));
        // });
    }
}