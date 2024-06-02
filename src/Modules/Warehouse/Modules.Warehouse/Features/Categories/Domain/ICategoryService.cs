namespace Modules.Warehouse.Features.Categories.Domain;

public interface ICategoryRepository
{
    bool CategoryExists(string categoryName);
}
