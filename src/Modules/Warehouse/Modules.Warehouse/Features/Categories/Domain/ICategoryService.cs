namespace Modules.Warehouse.Features.Categories.Domain;

internal interface ICategoryRepository
{
    bool CategoryExists(string categoryName);
}
