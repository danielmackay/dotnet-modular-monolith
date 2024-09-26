using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<AspireApp1_ApiService>("apiservice");

builder.AddProject<AspireApp1_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService);

builder.Build().Run();
