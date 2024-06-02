using Modules.Warehouse.Common.Interfaces;
using Modules.Warehouse.Features.Categories.Domain;

namespace Modules.Warehouse.Features.Categories;

public class CategoryRepository : ICategoryRepository
{
    private readonly IWarehouseDbContext _dbContext;

    public CategoryRepository(IWarehouseDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public bool CategoryExists(string categoryName)
    {
        return _dbContext.Categories.Any(c => c.Name == categoryName);
    }
}
