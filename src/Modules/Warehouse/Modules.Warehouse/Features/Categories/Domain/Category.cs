using Ardalis.GuardClauses;
using Common.SharedKernel.Domain.Base;
using Common.SharedKernel.Domain.Exceptions;

namespace Modules.Warehouse.Features.Categories.Domain;

public class Category : AggregateRoot<CategoryId>
{
    public string Name { get; private set; } = default!;

    private Category() { }

    // NOTE: Need to use a factory, as EF does not let owned entities (i.e Money & Sku) be passed via the constructor
    public static Category Create(string name, ICategoryRepository categoryRepository)
    {
        var category = new Category
        {
            Id = new CategoryId(Guid.NewGuid()),
        };

        category.UpdateName(name, categoryRepository);

        category.AddDomainEvent(new CategoryCreatedEvent(category.Id, category.Name));

        return category;
    }

    public void UpdateName(string name, ICategoryRepository categoryRepository)
    {
        Guard.Against.NullOrWhiteSpace(name);

        if (categoryRepository.CategoryExists(name))
            throw new DomainException($"Category {name} already exists");

        Name = name;
    }
}
