﻿namespace Modules.Warehouse.Products.Domain;

internal record Sku
{
    private const int DefaultLength = 8;

    public string Value { get; }

    private Sku(string value) => Value = value;

    public static Sku? Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        if (value.Length != DefaultLength)
            return null;

        return new Sku(value);
    }
}
