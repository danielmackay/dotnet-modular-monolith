namespace Common.SharedKernel.Domain.Interfaces;

public interface IStronglyTypedId<out T>
{
    T Value { get; }
}