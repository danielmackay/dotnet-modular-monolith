using Projects;

var builder = DistributedApplication.CreateBuilder();

builder
    .AddProject<WebApi>("api")
    .WithExternalHttpEndpoints();

var warehouseDb = builder
    .AddSqlServer("sql-warehouse")
    .AddDatabase("warehouse");

builder
    .AddSqlServer("sql-catalog")
    .AddDatabase("catalog");

builder.AddProject<MigrationService>("migrations")
    .WithReference(warehouseDb);

builder.Build().Run();
