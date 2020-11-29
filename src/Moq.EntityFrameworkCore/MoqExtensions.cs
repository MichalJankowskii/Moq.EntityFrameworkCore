namespace Moq.EntityFrameworkCore
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Microsoft.EntityFrameworkCore;
    using Moq.EntityFrameworkCore.DbAsyncQueryProvider;
    using Moq.Language;
    using Moq.Language.Flow;

    public static class MoqExtensions
    {
        public static IReturnsResult<T> ReturnsDbSet<T, TEntity>(this ISetup<T, DbSet<TEntity>> setupResult, IEnumerable<TEntity> entities, Mock<DbSet<TEntity>> dbSetMock = null) where T : class where TEntity : class
        {
            dbSetMock = dbSetMock ?? new Mock<DbSet<TEntity>>();

            ConfigureMock(dbSetMock, entities);

            return setupResult.Returns(dbSetMock.Object);
        }

        public static ISetupSequentialResult<DbSet<TEntity>> ReturnsDbSet<TEntity>(this ISetupSequentialResult<DbSet<TEntity>> setupResult, IEnumerable<TEntity> entities, Mock<DbSet<TEntity>> dbSetMock = null) where TEntity : class
        {
            dbSetMock = dbSetMock ?? new Mock<DbSet<TEntity>>();

            ConfigureMock(dbSetMock, entities);

            return setupResult.Returns(dbSetMock.Object);
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
            dbSetMock.As<IQueryable<TEntity>>().Setup(m => m.GetEnumerator()).Returns(() => entitiesAsQueryable.GetEnumerator());
        }
    }
}
