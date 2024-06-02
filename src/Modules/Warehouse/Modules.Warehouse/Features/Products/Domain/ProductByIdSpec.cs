using Ardalis.Specification;
using Common.SharedKernel.Domain.Identifiers;

namespace Modules.Warehouse.Features.Products.Domain;

public class ProductByIdSpec : Specification<Product>, ISingleResultSpecification<Product>
{
    public ProductByIdSpec(ProductId id) : base()
    {
        Query.Where(i => i.Id == id);
    }
}