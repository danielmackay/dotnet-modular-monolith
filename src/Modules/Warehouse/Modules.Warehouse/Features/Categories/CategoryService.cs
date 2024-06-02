using Modules.Warehouse.Common.Persistence;
using Modules.Warehouse.Features.Categories.Domain;

namespace Modules.Warehouse.Features.Categories;

public class CategoryRepository : ICategoryRepository
{
    private readonly WarehouseDbContext _dbContext;

    public CategoryRepository(WarehouseDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public bool CategoryExists(string categoryName)
    {
        return _dbContext.Categories.Any(c => c.Name == categoryName);
    }
}
