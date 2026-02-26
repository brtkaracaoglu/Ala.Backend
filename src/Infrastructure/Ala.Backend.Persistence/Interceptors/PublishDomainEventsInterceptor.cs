using Ala.Backend.Domain.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Ala.Backend.Persistence.Interceptors
{
    public class PublishDomainEventsInterceptor : SaveChangesInterceptor
    {
        private readonly IMediator _mediator;

        public PublishDomainEventsInterceptor(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result,
            CancellationToken cancellationToken = default)
        {
            var context = eventData.Context;
            if (context == null) return await base.SavedChangesAsync(eventData, result, cancellationToken);

            await PublishDomainEvents(context, cancellationToken);

            return await base.SavedChangesAsync(eventData, result, cancellationToken);
        }

        private async Task PublishDomainEvents(DbContext context, CancellationToken cancellationToken)
        {
            var domainEntities = context.ChangeTracker
                .Entries<IHasDomainEvents>()
                .Where(e => e.Entity.DomainEvents.Any())
                .ToList();

            var domainEvents = domainEntities
                .SelectMany(e => e.Entity.DomainEvents)
                .ToList();

            // Önce temizle (Sonsuz döngüyü önlemek için kritik)
            foreach (var entity in domainEntities)
            {
                entity.Entity.ClearDomainEvents();
            }

            // Sonra yayınla
            foreach (var domainEvent in domainEvents)
            {
                await _mediator.Publish(domainEvent, cancellationToken);
            }
        }
    }
}
