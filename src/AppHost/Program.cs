using Projects;

var builder = DistributedApplication.CreateBuilder();

var sqlServer = builder.AddSqlServer("sql-server");
var warehouseDb = sqlServer.AddDatabase("warehouse");
var catalogDb = sqlServer.AddDatabase("catalog");
var customersDb = sqlServer.AddDatabase("customers");
var ordersDb = sqlServer.AddDatabase("orders");

var migrationService = builder.AddProject<MigrationService>("migrations")
    .WithReference(warehouseDb)
    .WithReference(catalogDb)
    .WithReference(customersDb)
    .WithReference(ordersDb)
    .WaitFor(sqlServer);

builder
    .AddProject<WebApi>("api")
    .WithExternalHttpEndpoints()
    .WithReference(warehouseDb)
    .WithReference(catalogDb)
    .WithReference(customersDb)
    .WithReference(ordersDb)
    .WaitForCompletion(migrationService);

builder
    .Build()
    .Run();