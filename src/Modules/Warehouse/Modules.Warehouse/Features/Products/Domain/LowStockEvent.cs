using Common.SharedKernel.Domain.Base;
using Common.SharedKernel.Domain.Identifiers;

namespace Modules.Warehouse.Features.Products.Domain;

internal record LowStockEvent(ProductId ProductId) : DomainEvent;
