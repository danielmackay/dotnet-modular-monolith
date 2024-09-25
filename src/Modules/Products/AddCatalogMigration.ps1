dotnet ef migrations add Initial `
  --project ./Modules.Catalog `
  --startup-project ../../WebApi `
  --output-dir ./Common/Persistence/Migrations `
  --context CatalogDbContext
