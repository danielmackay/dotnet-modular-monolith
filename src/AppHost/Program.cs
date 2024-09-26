using Projects;

var builder = DistributedApplication.CreateBuilder(args);

builder
    .AddProject<WebApi>("apiservice")
    .WithExternalHttpEndpoints();


// builder
//     .AddContainer("mssql-server", opts =>
//     {
//         opts.EnvironmentVariables.Add("ACCEPT_EULA", "Y");
//         opts.EnvironmentVariables.Add("SA_PASSWORD", "Password123");
//         opts.EnvironmentVariables.Add("MSSQL_PID", "Developer");
//     });

// builder.AddProject<AspireApp1_Web>("webfrontend")
//     .WithExternalHttpEndpoints()
//     .WithReference(apiService);

builder.Build().Run();
