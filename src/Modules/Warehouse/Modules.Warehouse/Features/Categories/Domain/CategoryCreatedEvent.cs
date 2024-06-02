using Common.SharedKernel.Domain.Base;

namespace Modules.Warehouse.Features.Categories.Domain;

public record CategoryCreatedEvent(CategoryId Id, string Name) : DomainEvent;