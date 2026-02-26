using Ala.Backend.Application.Abstractions.Presentation;
using Ala.Backend.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Ala.Backend.Persistence.Interceptors
{
    public class AuditTrackableInterceptor : SaveChangesInterceptor
    {
        private readonly IRequestContext _requestContext;

        public AuditTrackableInterceptor(IRequestContext requestContext)
        {
            _requestContext = requestContext;
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
            InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            var context = eventData.Context;
            if (context == null) return base.SavingChangesAsync(eventData, result, cancellationToken);

            var userId = _requestContext.UserId;
            var now = DateTime.UtcNow;

            var trackableEntries = context.ChangeTracker.Entries<ITrackable>();
            foreach (var entry in trackableEntries)
            {
                if (entry.State == EntityState.Added)
                    entry.Entity.SetCreated(userId, now);
                else if (entry.State == EntityState.Modified)
                    entry.Entity.SetUpdated(userId, now);
            }

            var softDeleteEntries = context.ChangeTracker.Entries<ISoftDelete>();
            foreach (var entry in softDeleteEntries)
            {
                if (entry.State == EntityState.Deleted)
                {
                    entry.State = EntityState.Modified;

                    var deletedBy = _requestContext.Username ?? "System";
                    entry.Entity.MarkAsDeleted(deletedBy);
                }
            }

            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }
    }
}
