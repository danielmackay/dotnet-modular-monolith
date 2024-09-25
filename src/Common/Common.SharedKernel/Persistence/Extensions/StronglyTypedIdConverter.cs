using Common.SharedKernel.Domain.Interfaces;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Common.SharedKernel.Persistence.Extensions;

public class StronglyTypedIdConverter<TId, TValue> : ValueConverter<TId, TValue>
    where TId : IStronglyTypedId<TValue>
{
    public StronglyTypedIdConverter(ConverterMappingHints? mappingHints = null)
        : base(
            id => id.Value,
            value => (TId)Activator.CreateInstance(typeof(TId), value)!,
            mappingHints)
    {
    }
}
