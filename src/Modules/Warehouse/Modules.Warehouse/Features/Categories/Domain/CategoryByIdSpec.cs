﻿using Ardalis.Specification;

namespace Modules.Warehouse.Features.Categories.Domain;

public class CategoryByIdSpec : Specification<Category>, ISingleResultSpecification<Category>
{
    public CategoryByIdSpec(CategoryId id) : base()
    {
        Query.Where(i => i.Id == id);
    }
}