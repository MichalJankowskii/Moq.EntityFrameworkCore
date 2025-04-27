namespace Moq.EntityFrameworkCore.Dynamic
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Moq.Language.Flow;

    public static class GlobalFilterDbSetExtensionsDynamic
    {
        public static IReturnsResult<TContext> ReturnsDbSetWithGlobalFilterDynamic<TContext, TEntity>(
            this ISetup<TContext, DbSet<TEntity>> setup,
            IEnumerable<TEntity> entities,
            Func<TEntity, bool> filter
        )
            where TContext : DbContext
            where TEntity : class
        {
            var filtered = entities.Where(filter).ToList();
            return setup.ReturnsDbSetDynamic(filtered);
        }
    }
}
