namespace Moq.EntityFrameworkCore.Helpers
{
    using System.Collections.Generic;
    using System.Linq;
    using DbAsyncQueryProvider;
    using Language.Flow;
    using Microsoft.EntityFrameworkCore;

    public static class MoqExtensions
    {
        public static IReturnsResult<T> Returns<T, TEntity>(this ISetup<T, DbSet<TEntity>> setupResult, IEnumerable<TEntity> entities) where T : DbContext where TEntity : class
        {
            var entitiesAsQueryable = entities.AsQueryable();
            var dbSetMock = new Mock<DbSet<TEntity>>();

            dbSetMock.As<IAsyncEnumerable<TEntity>>()
               .Setup(m => m.GetEnumerator())
               .Returns(new InMemoryDbAsyncEnumerator<TEntity>(entitiesAsQueryable.GetEnumerator()));

            dbSetMock.As<IQueryable<TEntity>>()
                .Setup(m => m.Provider)
                .Returns(new InMemoryAsyncQueryProvider<TEntity>(entitiesAsQueryable.Provider));

            dbSetMock.As<IQueryable<TEntity>>().Setup(m => m.Expression).Returns(entitiesAsQueryable.Expression);
            dbSetMock.As<IQueryable<TEntity>>().Setup(m => m.ElementType).Returns(entitiesAsQueryable.ElementType);
            dbSetMock.As<IQueryable<TEntity>>().Setup(m => m.GetEnumerator()).Returns(entitiesAsQueryable.GetEnumerator());

            return setupResult.Returns(dbSetMock.Object);
        }
    }
}
