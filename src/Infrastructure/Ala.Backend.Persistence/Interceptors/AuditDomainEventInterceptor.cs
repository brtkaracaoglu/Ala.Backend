using Ala.Backend.Application.Abstractions.Presentation;
using Ala.Backend.Domain.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Ala.Backend.Persistence.Interceptors
{
    public class AuditDomainEventInterceptor : SaveChangesInterceptor
    {
        private readonly IRequestContext _requestContext;
        private readonly IMediator _mediator;

        public AuditDomainEventInterceptor(IRequestContext requestContext, IMediator mediator)
        {
            _requestContext = requestContext;
            _mediator = mediator;
        }

        // SAVE ÖNCESİ
        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
            InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            var context = eventData.Context;

            if (context == null)
                return await base.SavingChangesAsync(eventData, result, cancellationToken);

            ApplyAudit(context);

            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        // SAVE SONRASI
        public override async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result,
            CancellationToken cancellationToken = default)
        {
            var context = eventData.Context;

            if (context != null)
            {
                await PublishDomainEvents(context, cancellationToken);
            }

            return await base.SavedChangesAsync(eventData, result, cancellationToken);
        }

        private void ApplyAudit(DbContext context)
        {
            var entries = context.ChangeTracker
                .Entries<ITrackable>()
                .ToList();

            var userId = _requestContext.UserId;
            var now = DateTime.UtcNow;

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.SetCreated(userId, now);
                }

                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.SetUpdated(userId, now);
                }
            }
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

            foreach (var entity in domainEntities)
            {
                entity.Entity.ClearDomainEvents();
            }

            foreach (var domainEvent in domainEvents)
            {
                await _mediator.Publish(domainEvent, cancellationToken);
            }
        }
    }
}