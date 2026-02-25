using Ala.Backend.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Ala.Backend.Persistence.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void ApplySoftDeleteQueryFilter(this ModelBuilder builder)
        {
            var softDeleteEntities = builder.Model
                .GetEntityTypes()
                .Where(t => typeof(ISoftDelete).IsAssignableFrom(t.ClrType));

            foreach (var entityType in softDeleteEntities)
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");

                var propertyMethod = typeof(EF).GetMethod("Property")!
                    .MakeGenericMethod(typeof(bool));

                var isDeletedProperty = Expression.Call(
                    propertyMethod,
                    parameter,
                    Expression.Constant("IsDeleted"));

                var compareExpression = Expression.Equal(
                    isDeletedProperty,
                    Expression.Constant(false));

                var lambda = Expression.Lambda(
                    compareExpression,
                    parameter);

                builder.Entity(entityType.ClrType)
                    .HasQueryFilter(lambda);
            }
        }
    }
}