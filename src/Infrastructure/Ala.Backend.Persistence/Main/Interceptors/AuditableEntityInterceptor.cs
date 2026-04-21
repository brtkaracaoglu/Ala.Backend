using Ala.Backend.Application.Abstractions.Presentation;
using Ala.Backend.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Ala.Backend.Persistence.Main.Interceptors
{
    public sealed class AuditableEntityInterceptor : SaveChangesInterceptor
    {
        private readonly IRequestContext _requestContext;

        public AuditableEntityInterceptor(IRequestContext requestContext)
        {
            _requestContext = requestContext;
        }

        public override InterceptionResult<int> SavingChanges(
            DbContextEventData eventData,
            InterceptionResult<int> result)
        {
            ApplyAudit(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            ApplyAudit(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void ApplyAudit(DbContext? dbContext)
        {
            if (dbContext is null)
                return;

            var now = DateTime.UtcNow;
            var userId = _requestContext.UserId;

            var entries = dbContext.ChangeTracker.Entries<ITrackable>();

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
        }
    }
}