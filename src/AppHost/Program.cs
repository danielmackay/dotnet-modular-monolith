using Projects;

var builder = DistributedApplication.CreateBuilder();

builder
    .AddProject<WebApi>("api")
    .WithExternalHttpEndpoints();

var warehouseDb = builder
    .AddSqlServer("sql-warehouse")
    // .WithImage("mcr.microsoft.com/mssql/server:2022-CU14-ubuntu-22.04")
    .AddDatabase("warehouse");

builder
    .AddSqlServer("sql-catalog")
    .AddDatabase("catalog");

builder.AddProject<MigrationService>("migrations")
    .WithReference(warehouseDb);

builder.Build().Run();
