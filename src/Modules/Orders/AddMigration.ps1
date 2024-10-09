dotnet ef migrations add Initial `
  --project ./Modules.Orders `
  --startup-project ../../WebApi `
  --output-dir ./Common/Persistence/Migrations `
  --context OrdersDbContext
