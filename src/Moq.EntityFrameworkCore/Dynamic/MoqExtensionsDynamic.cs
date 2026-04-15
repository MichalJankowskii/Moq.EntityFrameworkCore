namespace Moq.EntityFrameworkCore.Dynamic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Moq.EntityFrameworkCore.DbAsyncQueryProvider;
    using Moq.Language;
    using Moq.Language.Flow;

    public static class MoqExtensionsDynamic
    {
        public static IReturnsResult<T> ReturnsDbSetDynamic<T, TEntity>(this ISetupGetter<T, DbSet<TEntity>> setupResult, IEnumerable<TEntity> entities, Mock<DbSet<TEntity>>? dbSetMock = null) where T : class where TEntity : class
        {
            dbSetMock ??= new Mock<DbSet<TEntity>>();

            ConfigureMockDynamic(dbSetMock, entities);

            return setupResult.Returns(() => dbSetMock.Object);
        }

        public static IReturnsResult<T> ReturnsDbSetDynamic<T, TEntity>(this ISetupGetter<T, DbSet<TEntity>> setupResult, IEnumerable<TEntity> entities, Func<TEntity, object?[]> findByKeyExpression, Mock<DbSet<TEntity>>? dbSetMock = null) where T : class where TEntity : class
        {
            dbSetMock ??= new Mock<DbSet<TEntity>>();

            ConfigureMockDynamic(dbSetMock, entities, findByKeyExpression);

            return setupResult.Returns(() => dbSetMock.Object);
        }

        public static IReturnsResult<T> ReturnsDbSetDynamic<T, TEntity>(this ISetup<T, DbSet<TEntity>> setupResult, IEnumerable<TEntity> entities, Mock<DbSet<TEntity>>? dbSetMock = null) where T : class where TEntity : class
        {
            dbSetMock ??= new Mock<DbSet<TEntity>>();

            ConfigureMockDynamic(dbSetMock, entities);

            return setupResult.Returns(() => dbSetMock.Object);
        }

        public static IReturnsResult<T> ReturnsDbSetDynamic<T, TEntity>(this ISetup<T, DbSet<TEntity>> setupResult, IEnumerable<TEntity> entities, Func<TEntity, object?[]> findByKeyExpression, Mock<DbSet<TEntity>>? dbSetMock = null) where T : class where TEntity : class
        {
            dbSetMock ??= new Mock<DbSet<TEntity>>();

            ConfigureMockDynamic(dbSetMock, entities, findByKeyExpression);

            return setupResult.Returns(() => dbSetMock.Object);
        }

        public static ISetupSequentialResult<DbSet<TEntity>> ReturnsDbSetDynamic<TEntity>(this ISetupSequentialResult<DbSet<TEntity>> setupResult, IEnumerable<TEntity> entities, Mock<DbSet<TEntity>>? dbSetMock = null) where TEntity : class
        {
            dbSetMock ??= new Mock<DbSet<TEntity>>();

            ConfigureMockDynamic(dbSetMock, entities);

            return setupResult.Returns(() => dbSetMock.Object);
        }

        public static ISetupSequentialResult<DbSet<TEntity>> ReturnsDbSetDynamic<TEntity>(this ISetupSequentialResult<DbSet<TEntity>> setupResult, IEnumerable<TEntity> entities, Func<TEntity, object?[]> findByKeyExpression, Mock<DbSet<TEntity>>? dbSetMock = null) where TEntity : class
        {
            dbSetMock ??= new Mock<DbSet<TEntity>>();

            ConfigureMockDynamic(dbSetMock, entities, findByKeyExpression);

            return setupResult.Returns(() => dbSetMock.Object);
        }

        /// <summary>
        /// Configures a Mock for a <see cref="DbSet{TEntity}"/> so that it can be queryable via LINQ
        /// </summary>
        public static void ConfigureMockDynamic<TEntity>(Mock dbSetMock, IEnumerable<TEntity> entities) where TEntity : class
        {
            var entitiesAsQueryable = entities.AsQueryable();

            dbSetMock.As<IAsyncEnumerable<TEntity>>()
                .Setup(m => m.GetAsyncEnumerator(CancellationToken.None))
                .Returns(() => new InMemoryDbAsyncEnumerator<TEntity>(entitiesAsQueryable.GetEnumerator()));

            dbSetMock.As<IQueryable<TEntity>>()
                .Setup(m => m.Provider)
                .Returns(new InMemoryAsyncQueryProvider<TEntity>(entitiesAsQueryable.Provider));

            dbSetMock.As<IQueryable<TEntity>>().Setup(m => m.Expression).Returns(entitiesAsQueryable.Expression);
            dbSetMock.As<IQueryable<TEntity>>().Setup(m => m.ElementType).Returns(entitiesAsQueryable.ElementType);
            dbSetMock.As<IQueryable<TEntity>>().Setup(m => m.GetEnumerator()).Returns(() => entitiesAsQueryable.GetEnumerator());
        }

        /// <summary>
        /// Configures a Mock for a <see cref="DbSet{TEntity}"/> so that it can be queryable via LINQ
        /// and mocks <see cref="DbSet{TEntity}.FindAsync(object?[])"/> using the provided key expression.
        /// </summary>
        /// <param name="dbSetMock">The mock to configure.</param>
        /// <param name="entities">The entities to use as the data source.</param>
        /// <param name="findByKeyExpression">
        /// A delegate that extracts the key values from an entity. The values must be returned in the same
        /// order as they are passed to <see cref="DbSet{TEntity}.FindAsync(object?[])"/>, matching the
        /// order of the key properties in the EF Core model.
        /// </param>
        public static void ConfigureMockDynamic<TEntity>(Mock<DbSet<TEntity>> dbSetMock, IEnumerable<TEntity> entities, Func<TEntity, object?[]> findByKeyExpression) where TEntity : class
        {
            ArgumentNullException.ThrowIfNull(findByKeyExpression);

            // Materialise once so that both the LINQ queryable and FindAsync lambdas
            // operate on the same snapshot and single-use enumerables are safe.
            var entitiesList = entities.ToList();

            ConfigureMockDynamic((Mock)dbSetMock, entitiesList);

            dbSetMock
                .Setup(m => m.FindAsync(It.IsAny<object?[]?>()))
                .Returns<object?[]?>(keyValues =>
                {
                    if (keyValues == null || keyValues.Length == 0) return ValueTask.FromResult<TEntity?>(default);
                    return ValueTask.FromResult(entitiesList.FirstOrDefault(e => findByKeyExpression(e).SequenceEqual(keyValues!)));
                });

            dbSetMock
                .Setup(m => m.FindAsync(It.IsAny<object?[]?>(), It.IsAny<CancellationToken>()))
                .Returns<object?[]?, CancellationToken>((keyValues, _) =>
                {
                    if (keyValues == null || keyValues.Length == 0) return ValueTask.FromResult<TEntity?>(default);
                    return ValueTask.FromResult(entitiesList.FirstOrDefault(e => findByKeyExpression(e).SequenceEqual(keyValues!)));
                });
        }
    }
}
