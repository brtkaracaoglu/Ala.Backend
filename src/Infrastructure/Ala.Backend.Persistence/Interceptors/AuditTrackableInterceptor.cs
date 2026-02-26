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

            var entries = context.ChangeTracker.Entries<ITrackable>();

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.SetCreated(userId, now);
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.SetUpdated(userId, now);
                }
            }

            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }
    }
}
