using Common.SharedKernel.Domain.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Common.SharedKernel.Persistence.Extensions;

public static class PropertyBuilderExtensions
{
    public static PropertyBuilder<TId> HasStronglyTypedId<TId, TValue>(this PropertyBuilder<TId> propertyBuilder)
        where TId : IStronglyTypedId<TValue>
    {
        return propertyBuilder.HasConversion(new StronglyTypedIdConverter<TId, TValue>());
    }
}
