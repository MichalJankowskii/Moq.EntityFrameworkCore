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

    /// <summary>
    /// Extension methods for configuring and mocking a <see cref="DbSet{TEntity}"/>
    /// </summary>
    public static class MoqExtensions
    {
        /// <summary>
        /// Returns a configured DbSet mock with the specified entities.
        /// </summary>
        /// <typeparam name="T">The type of the mock setup interface.</typeparam>
        /// <typeparam name="TEntity">The entity type for the DbSet.</typeparam>
        /// <param name="setupResult">The mock setup instance for DbSet retrieval.</param>
        /// <param name="entities">The collection of entities to be used for configuring the mock.</param>
        public static IReturnsResult<T> ReturnsDbSet<T, TEntity>(
            this ISetupGetter<T, DbSet<TEntity>> setupResult,
            IEnumerable<TEntity> entities)
            where T : class
            where TEntity : class
        {
            return ReturnsDbSet(setupResult, entities, new Mock<DbSet<TEntity>>());
        }

        /// <summary>
        /// Returns a configured DbSet mock with the specified entities.
        /// </summary>
        /// <typeparam name="T">The type of the mock setup interface.</typeparam>
        /// <typeparam name="TEntity">The entity type for the DbSet.</typeparam>
        /// <param name="setupResult">The mock setup instance for DbSet retrieval.</param>
        /// <param name="entities">The collection of entities to be used for configuring the mock.</param>
        /// <param name="dbSetMock">The Mock instance representing the DbSet.</param>
        public static IReturnsResult<T> ReturnsDbSet<T, TEntity>(
            this ISetupGetter<T, DbSet<TEntity>> setupResult,
            IEnumerable<TEntity> entities,
            Mock<DbSet<TEntity>> dbSetMock)
            where T : class
            where TEntity : class
        {
            ConfigureMock(dbSetMock, entities);

            return setupResult.Returns(dbSetMock.Object);
        }

        /// <summary>
        /// Returns a configured DbSet mock with the specified entities.
        /// </summary>
        /// <typeparam name="T">The type of the mock setup interface.</typeparam>
        /// <typeparam name="TEntity">The entity type for the DbSet.</typeparam>
        /// <param name="setupResult">The mock setup instance for DbSet retrieval.</param>
        /// <param name="entities">The collection of entities to be used for configuring the mock.</param>
        public static IReturnsResult<T> ReturnsDbSet<T, TEntity>(
            this ISetup<T, DbSet<TEntity>> setupResult,
            IEnumerable<TEntity> entities)
            where T : class
            where TEntity : class
        {
            return ReturnsDbSet(setupResult, entities, new Mock<DbSet<TEntity>>());
        }

        /// <summary>
        /// Returns a configured DbSet mock with the specified entities.
        /// </summary>
        /// <typeparam name="T">The type of the mock setup interface.</typeparam>
        /// <typeparam name="TEntity">The entity type for the DbSet.</typeparam>
        /// <param name="setupResult">The mock setup instance for DbSet retrieval.</param>
        /// <param name="entities">The collection of entities to be used for configuring the mock.</param>
        /// <param name="dbSetMock">The Mock instance representing the DbSet.</param>
        public static IReturnsResult<T> ReturnsDbSet<T, TEntity>(
            this ISetup<T, DbSet<TEntity>> setupResult,
            IEnumerable<TEntity> entities,
            Mock<DbSet<TEntity>> dbSetMock)
            where T : class
            where TEntity : class
        {
            ConfigureMock(dbSetMock, entities);

            return setupResult.Returns(dbSetMock.Object);
        }

        /// <summary>
        /// Returns a configured DbSet mock with the specified entities.
        /// </summary>
        /// <typeparam name="T">The type of the mock setup interface.</typeparam>
        /// <typeparam name="TEntity">The entity type for the DbSet.</typeparam>
        /// <param name="setupResult">The mock setup instance for DbSet retrieval.</param>
        /// <param name="entities">The collection of entities to be used for configuring the mock.</param>
        public static ISetupSequentialResult<DbSet<TEntity>> ReturnsDbSet<TEntity>(
            this ISetupSequentialResult<DbSet<TEntity>> setupResult,
            IEnumerable<TEntity> entities)
            where TEntity : class
        {
            return ReturnsDbSet(setupResult, entities, new Mock<DbSet<TEntity>>());
        }

        /// <summary>
        /// Returns a configured DbSet mock with the specified entities.
        /// </summary>
        /// <typeparam name="T">The type of the mock setup interface.</typeparam>
        /// <typeparam name="TEntity">The entity type for the DbSet.</typeparam>
        /// <param name="setupResult">The mock setup instance for DbSet retrieval.</param>
        /// <param name="entities">The collection of entities to be used for configuring the mock.</param>
        /// <param name="dbSetMock">The Mock instance representing the DbSet.</param>
        public static ISetupSequentialResult<DbSet<TEntity>> ReturnsDbSet<TEntity>(
            this ISetupSequentialResult<DbSet<TEntity>> setupResult,
            IEnumerable<TEntity> entities,
            Mock<DbSet<TEntity>> dbSetMock)
            where TEntity : class
        {
            ConfigureMock(dbSetMock, entities);

            return setupResult.Returns(dbSetMock.Object);
        }

        /// <summary>
        /// Configures a Mock for a <see cref="DbSet{TEntity}"/> to make its collection of entities queriable.
        /// </summary>
        /// <typeparam name="TEntity">The entity type for the DbSet.</typeparam>
        /// <param name="dbSetMock">The Mock instance representing the DbSet.</param>
        /// <param name="entities">The collection of entities to be made queriable.</param>
        private static void ConfigureMock<TEntity>(
            Mock<DbSet<TEntity>> dbSetMock,
            IEnumerable<TEntity> entities)
            where TEntity : class
        {
            var entitiesAsQueryable = entities.AsQueryable();

            dbSetMock.As<IAsyncEnumerable<TEntity>>()
                .Setup(m => m.GetAsyncEnumerator(CancellationToken.None))
                .Returns(new InMemoryDbAsyncEnumerator<TEntity>(entitiesAsQueryable.GetEnumerator()));

            dbSetMock.As<IQueryable<TEntity>>()
                .Setup(m => m.Provider)
                .Returns(new InMemoryAsyncQueryProvider<TEntity>(entitiesAsQueryable.Provider));

            dbSetMock.As<IQueryable<TEntity>>()
                .Setup(m => m.Expression)
                .Returns(entitiesAsQueryable.Expression);

            dbSetMock.As<IQueryable<TEntity>>()
                .Setup(m => m.ElementType)
                .Returns(entitiesAsQueryable.ElementType);

            dbSetMock.As<IQueryable<TEntity>>()
                .Setup(m => m.GetEnumerator())
                .Returns(entitiesAsQueryable.GetEnumerator);
        }
    }
}
