using Ala.Backend.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Ala.Backend.Persistence.Main.Extensions
{
    public static class SoftDeleteModelBuilderExtensions
    {
        public static void ApplyGlobalSoftDelete(this ModelBuilder modelBuilder)
        {
            var clrTypes = modelBuilder.Model
                .GetEntityTypes()
                .Where(x => typeof(ISoftDelete).IsAssignableFrom(x.ClrType))
                .Select(x => x.ClrType)
                .ToList();

            var propertyMethod = typeof(EF)
                .GetMethod(nameof(EF.Property))!
                .MakeGenericMethod(typeof(bool));

            foreach (var clrType in clrTypes)
            {
                var parameter = Expression.Parameter(clrType, "e");

                var isDeletedProperty = Expression.Call(
                    propertyMethod,
                    parameter,
                    Expression.Constant(nameof(ISoftDelete.IsDeleted)));

                var compareExpression = Expression.Equal(
                    isDeletedProperty,
                    Expression.Constant(false));

                var lambda = Expression.Lambda(compareExpression, parameter);

                modelBuilder.Entity(clrType).HasQueryFilter(lambda);
            }
        }
    }
}