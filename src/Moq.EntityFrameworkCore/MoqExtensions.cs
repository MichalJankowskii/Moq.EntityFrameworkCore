namespace Moq.EntityFrameworkCore
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

    public static class MoqExtensions
    {
        public static IReturnsResult<T> ReturnsDbSet<T, TEntity>(this ISetupGetter<T, DbSet<TEntity>> setupResult, IEnumerable<TEntity> entities, Mock<DbSet<TEntity>>? dbSetMock = null, Func<TEntity, object[]>? findByKeyExpression = null) where T : class where TEntity : class
        {
            dbSetMock = dbSetMock ?? new Mock<DbSet<TEntity>>();

            ConfigureMock(dbSetMock, entities, findByKeyExpression);

            return setupResult.Returns(dbSetMock.Object);
        }

        public static IReturnsResult<T> ReturnsDbSet<T, TEntity>(this ISetup<T, DbSet<TEntity>> setupResult, IEnumerable<TEntity> entities, Mock<DbSet<TEntity>>? dbSetMock = null, Func<TEntity, object[]>? findByKeyExpression = null) where T : class where TEntity : class
        {
            dbSetMock = dbSetMock ?? new Mock<DbSet<TEntity>>();

            ConfigureMock(dbSetMock, entities, findByKeyExpression);

            return setupResult.Returns(dbSetMock.Object);
        }

        public static ISetupSequentialResult<DbSet<TEntity>> ReturnsDbSet<TEntity>(this ISetupSequentialResult<DbSet<TEntity>> setupResult, IEnumerable<TEntity> entities, Mock<DbSet<TEntity>>? dbSetMock = null, Func<TEntity, object[]>? findByKeyExpression = null) where TEntity : class
        {
            dbSetMock = dbSetMock ?? new Mock<DbSet<TEntity>>();

            ConfigureMock(dbSetMock, entities, findByKeyExpression);

            return setupResult.Returns(dbSetMock.Object);
        }

        /// <summary>
        /// Configures a Mock for a <see cref="DbSet{TEntity}"/> so that it can be queryable via LINQ
        /// </summary>
        public static void ConfigureMock<TEntity>(Mock dbSetMock, IEnumerable<TEntity> entities) where TEntity : class
        {
            var entitiesAsQueryable = entities.AsQueryable();

            dbSetMock.As<IAsyncEnumerable<TEntity>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
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
        /// </summary>
        public static void ConfigureMock<TEntity>(Mock<DbSet<TEntity>> dbSetMock, IEnumerable<TEntity> entities, Func<TEntity, object[]>? findByKeyExpression = null) where TEntity : class
        {
            ConfigureMock((Mock)dbSetMock, entities);

            if (findByKeyExpression is null) return;

            dbSetMock
                .Setup(m => m.FindAsync(It.IsAny<object?[]?>()))
                .Returns<object?[]?>(keyValues =>
                {
                    if (keyValues == null || keyValues.Length == 0) return ValueTask.FromResult<TEntity?>(default);
                    return ValueTask.FromResult(entities.FirstOrDefault(e => findByKeyExpression(e).SequenceEqual(keyValues!)));
                });

            dbSetMock
                .Setup(m => m.FindAsync(It.IsAny<object?[]?>(), It.IsAny<CancellationToken>()))
                .Returns<object?[]?, CancellationToken>((keyValues, _) =>
                {
                    if (keyValues == null || keyValues.Length == 0) return ValueTask.FromResult<TEntity?>(default);
                    return ValueTask.FromResult(entities.FirstOrDefault(e => findByKeyExpression(e).SequenceEqual(keyValues!)));
                });
        }
    }
}
