using Projects;

var builder = DistributedApplication.CreateBuilder();

// TODO: Figure out how to keep these running after the AppHost shuts down
var warehouseDb = builder
    .AddSqlServer("warehouse-sql")
    .AddDatabase("warehouse");

var catalogDb = builder
    .AddSqlServer("catalog-sql")
    .AddDatabase("catalog");

builder.AddProject<MigrationService>("migrations")
    .WithReference(warehouseDb)
    .WithReference(catalogDb);

builder
    .AddProject<WebApi>("api")
    .WithExternalHttpEndpoints()
    .WithReference(warehouseDb)
    .WithReference(catalogDb);

builder
    .Build()
    .Run();
