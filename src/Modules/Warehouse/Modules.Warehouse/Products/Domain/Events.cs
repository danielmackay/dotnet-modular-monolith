using Common.SharedKernel.Domain.Interfaces;

namespace Modules.Warehouse.Products.Domain;

internal record ProductCreatedEvent(Product Product) : IDomainEvent;

internal record LowStockEvent(Product Product) : IDomainEvent;