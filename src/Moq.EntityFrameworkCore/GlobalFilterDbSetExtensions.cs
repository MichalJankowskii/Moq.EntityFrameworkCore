namespace Moq.EntityFrameworkCore
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Moq.Language.Flow;

    public static class GlobalFilterDbSetExtensions
    {
        public static IReturnsResult<TContext> ReturnsDbSetWithGlobalFilter<TContext, TEntity>(
            this ISetup<TContext, DbSet<TEntity>> setup,
            IEnumerable<TEntity> entities,
            Func<TEntity, bool> filter
        )
            where TContext : DbContext
            where TEntity : class
        {
            var filtered = entities.Where(filter).ToList();
            return setup.ReturnsDbSet(filtered);
        }
    }
}
