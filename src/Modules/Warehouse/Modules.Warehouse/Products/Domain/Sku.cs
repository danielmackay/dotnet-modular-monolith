namespace Modules.Warehouse.Products.Domain;

internal record Sku : ValueObject
{
    internal const int DefaultLength = 8;

    public string Value { get; }

    private Sku(string value) => Value = value;

    public static Sku Create(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        ArgumentOutOfRangeException.ThrowIfNotEqual(value.Length, DefaultLength);

        return new Sku(value);
    }
}
