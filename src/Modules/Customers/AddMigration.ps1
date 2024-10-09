dotnet ef migrations add Initial `
  --project ./Modules.Customers `
  --startup-project ../../WebApi `
  --output-dir ./Common/Persistence/Migrations `
  --context CustomersDbContext
