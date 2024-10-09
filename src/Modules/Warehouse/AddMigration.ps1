dotnet ef migrations add Initial `
  --project ./Modules.Warehouse `
  --startup-project ../../WebApi `
  --output-dir ./Common/Persistence/Migrations `
  --context WarehouseDbContext
