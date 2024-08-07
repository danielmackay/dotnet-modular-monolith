﻿// using Microsoft.EntityFrameworkCore;
// using Modules.Warehouse.Common.Persistence;
// using Modules.Warehouse.Products.Domain;
//
// namespace Modules.Warehouse.Products;
//
// internal class ProductRepository : IProductRepository
// {
//     private readonly WarehouseDbContext _dbContext;
//
//     public ProductRepository(WarehouseDbContext dbContext)
//     {
//         _dbContext = dbContext;
//     }
//
//     public async Task<bool> SkuExistsAsync(Sku sku)
//     {
//         return await _dbContext.Products.AnyAsync(p => p.Sku == sku);
//     }
//
//     public bool SkuExists(Sku sku)
//     {
//         return _dbContext.Products.Any(p => p.Sku == sku);
//     }
// }
