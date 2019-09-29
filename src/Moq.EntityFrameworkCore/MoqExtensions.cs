namespace Moq.EntityFrameworkCore
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Microsoft.EntityFrameworkCore;
    using Moq.EntityFrameworkCore.DbAsyncQueryProvider;
    using Moq.Language.Flow;

    public static class MoqExtensions
    {
        public static IReturnsResult<T> ReturnsDbSet<T, TEntity>(this ISetup<T, DbSet<TEntity>> setupResult, IEnumerable<TEntity> entities, Mock<DbSet<TEntity>> dbSetMock = null) where T : DbContext where TEntity : class
        {
            dbSetMock = dbSetMock ?? new Mock<DbSet<TEntity>>();

            ConfigureMock(dbSetMock, entities);

            return setupResult.Returns(dbSetMock.Object);
        }

        [Obsolete("Use ReturnsDbSet<T, TEntity> instead")]
        public static IReturnsResult<T> ReturnsDbQuery<T, TEntity>(this ISetup<T, DbQuery<TEntity>> setupResult, IEnumerable<TEntity> entities, Mock<DbQuery<TEntity>> dbQueryMock = null) where T : DbContext where TEntity : class
        {
            dbQueryMock = dbQueryMock ?? new Mock<DbQuery<TEntity>>();

            ConfigureMock(dbQueryMock, entities);

            return setupResult.Returns(dbQueryMock.Object);
        }

        /// <summary>
        /// Configures a Mock for a <see cref="DbSet{TEntity}"/> or a <see cref="DbQuery{TQuery}"/> so that it can be queriable via LINQ
        /// </summary>
        private static void ConfigureMock<TEntity>(Mock dbSetMock, IEnumerable<TEntity> entities) where TEntity : class
        {
            var entitiesAsQueryable = entities.AsQueryable();

            dbSetMock.As<IAsyncEnumerable<TEntity>>()
               .Setup(m => m.GetAsyncEnumerator(CancellationToken.None))
               .Returns(new InMemoryDbAsyncEnumerator<TEntity>(entitiesAsQueryable.GetEnumerator()));

            dbSetMock.As<IQueryable<TEntity>>()
                .Setup(m => m.Provider)
                .Returns(new InMemoryAsyncQueryProvider<TEntity>(entitiesAsQueryable.Provider));

            dbSetMock.As<IQueryable<TEntity>>().Setup(m => m.Expression).Returns(entitiesAsQueryable.Expression);
            dbSetMock.As<IQueryable<TEntity>>().Setup(m => m.ElementType).Returns(entitiesAsQueryable.ElementType);
            dbSetMock.As<IQueryable<TEntity>>().Setup(m => m.GetEnumerator()).Returns(entitiesAsQueryable.GetEnumerator());
        }
    }
}
