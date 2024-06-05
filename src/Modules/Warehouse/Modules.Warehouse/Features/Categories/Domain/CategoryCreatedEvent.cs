using Common.SharedKernel.Domain.Base;

namespace Modules.Warehouse.Features.Categories.Domain;

internal record CategoryCreatedEvent(CategoryId Id, string Name) : DomainEvent;
