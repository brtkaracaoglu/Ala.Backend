using Ala.Backend.Application.Abstractions.Presentation;
using Ala.Backend.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Ala.Backend.Persistence.Main.Interceptors
{
    public sealed class SoftDeleteInterceptor : SaveChangesInterceptor
    {
        private readonly IRequestContext _requestContext;

        public SoftDeleteInterceptor(IRequestContext requestContext)
        {
            _requestContext = requestContext;
        }

        public override InterceptionResult<int> SavingChanges(
            DbContextEventData eventData,
            InterceptionResult<int> result)
        {
            ApplySoftDelete(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            ApplySoftDelete(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void ApplySoftDelete(DbContext? dbContext)
        {
            if (dbContext is null)
                return;

            var userId = _requestContext.UserId;
            var entries = dbContext.ChangeTracker.Entries<ISoftDelete>();

            foreach (var entry in entries)
            {
                if (entry.State != EntityState.Deleted)
                    continue;

                entry.State = EntityState.Modified;
                entry.Entity.MarkAsDeleted(userId);
            }
        }
    }
}