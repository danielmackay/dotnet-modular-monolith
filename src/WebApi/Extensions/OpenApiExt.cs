namespace WebApi.Extensions;

public static class OpenApiExt
{
    public static void AddSwagger(this IServiceCollection services)
    {
        services.AddMicrosoftOpenApi();
    }

    private static void AddMicrosoftOpenApi(this IServiceCollection services)
    {
        services.AddOpenApi();
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

public static class ScalarExtensions
{
    public static IEndpointConventionBuilder MapScalarUi(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/scalar/{documentName}", (string documentName) => Results.Content($$"""
              <!doctype html>
              <html>
              <head>
                  <title>Scalar API Reference -- {{documentName}}</title>
                  <meta charset="utf-8" />
                  <meta
                  name="viewport"
                  content="width=device-width, initial-scale=1" />
              </head>
              <body>
                  <script
                  id="api-reference"
                  data-url="/openapi/{{documentName}}.json"></script>
                  <script>
                  var configuration = {
                      theme: 'purple',
                  }
              
                  document.getElementById('api-reference').dataset.configuration =
                      JSON.stringify(configuration)
                  </script>
                  <script src="https://cdn.jsdelivr.net/npm/@scalar/api-reference"></script>
              </body>
              </html>
              """, "text/html")).ExcludeFromDescription();
    }
}