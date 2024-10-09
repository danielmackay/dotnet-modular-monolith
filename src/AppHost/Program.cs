using Projects;

var builder = DistributedApplication.CreateBuilder();

// TODO: Figure out how to keep these running after the AppHost shuts down
// TODO: Perhaps we can store the SQL Server in a variable to add multiple DB's to it?
var warehouseDb = builder
    .AddSqlServer("warehouse-sql")
    .AddDatabase("warehouse");

var catalogDb = builder
    .AddSqlServer("catalog-sql")
    .AddDatabase("catalog");

var customersDb = builder
    .AddSqlServer("customers-sql")
    .AddDatabase("customers");

var ordersDb = builder
    .AddSqlServer("orders-sql")
    .AddDatabase("orders");

builder.AddProject<MigrationService>("migrations")
    .WithReference(warehouseDb)
    .WithReference(catalogDb)
    .WithReference(customersDb)
    .WithReference(ordersDb);

builder
    .AddProject<WebApi>("api")
    .WithExternalHttpEndpoints()
    .WithReference(warehouseDb)
    .WithReference(catalogDb)
    .WithReference(customersDb)
    .WithReference(ordersDb);

builder
    .Build()
    .Run();
