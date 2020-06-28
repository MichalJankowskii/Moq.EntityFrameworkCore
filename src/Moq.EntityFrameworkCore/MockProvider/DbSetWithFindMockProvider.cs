namespace Moq.EntityFrameworkCore.MockProvider
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Microsoft.EntityFrameworkCore;

    public class DbSetWithFindMockProvider
    {
        private DbSetWithFindMockProvider()
        {
        }

        public DbSetWithFindMockProvider Instance => new DbSetWithFindMockProvider();

        public Mock<DbSet<TEntity>> GenerateMock<TEntity>(ICollection<TEntity> entities, Func<TEntity, object[], bool> findFunction, Mock<DbSet<TEntity>> predefinedMock = null)
            where TEntity : class
        {
            var mock = predefinedMock ?? new Mock<DbSet<TEntity>>();

            mock
                .Setup(dbSet => dbSet.Update(It.IsAny<TEntity>()))
                .Callback<TEntity>(entity =>
                {
                    entities.Remove(entity);
                    entities.Add(entity);
                });

            mock
                .Setup(dbSet => dbSet.Add(It.IsAny<TEntity>()))
                .Callback<TEntity>(entities.Add);

            mock
                .Setup(dbSet => dbSet.Remove(It.IsAny<TEntity>()))
                .Callback<TEntity>(entity => entities.Remove(entity));

            mock
                .Setup(dbSet => dbSet.Find(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
                .Returns((object[] parameters, CancellationToken cancellationToken)
                    => entities.SingleOrDefault(entity => findFunction(entity, parameters)));

            mock
                .Setup(dbSet => dbSet.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((object[] parameters, CancellationToken cancellationToken)
                    => entities.SingleOrDefault(entity => findFunction(entity, parameters)));

            return mock;
        }
    }
}
