namespace Moq.EntityFrameworkCore.Tests
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Xunit;

    public class FindAsyncTests
    {
        [Fact]
        public async Task FindAsync_WithMatchingKey_ReturnsCorrectEntity()
        {
            // Arrange
            var entities = new List<TestEntity>
            {
                new() { Id = 1, Name = "Alice" },
                new() { Id = 2, Name = "Bob" },
                new() { Id = 3, Name = "Charlie" }
            };

            var dbSetMock = new Mock<DbSet<TestEntity>>();
            MoqExtensions.ConfigureMock(dbSetMock, entities, findByKeyExpression: e => e.Id);

            // Act
            var result = await dbSetMock.Object.FindAsync(2);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Bob", result.Name);
        }

        [Fact]
        public async Task FindAsync_WithNonMatchingKey_ReturnsNull()
        {
            // Arrange
            var entities = new List<TestEntity>
            {
                new() { Id = 1, Name = "Alice" },
                new() { Id = 2, Name = "Bob" }
            };

            var dbSetMock = new Mock<DbSet<TestEntity>>();
            MoqExtensions.ConfigureMock(dbSetMock, entities, findByKeyExpression: e => e.Id);

            // Act
            var result = await dbSetMock.Object.FindAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task FindAsync_WithCancellationToken_ReturnsCorrectEntity()
        {
            // Arrange
            var entities = new List<TestEntity>
            {
                new() { Id = 1, Name = "Alice" },
                new() { Id = 2, Name = "Bob" },
                new() { Id = 3, Name = "Charlie" }
            };

            var dbSetMock = new Mock<DbSet<TestEntity>>();
            MoqExtensions.ConfigureMock(dbSetMock, entities, findByKeyExpression: e => e.Id);

            using var cts = new CancellationTokenSource();

            // Act
            var result = await dbSetMock.Object.FindAsync(new object[] { 3 }, cts.Token);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Charlie", result.Name);
        }

        [Fact]
        public async Task FindAsync_WithoutFindByKeyExpression_ReturnsDefault()
        {
            // Arrange
            var entities = new List<TestEntity>
            {
                new() { Id = 1, Name = "Alice" }
            };

            var dbSetMock = new Mock<DbSet<TestEntity>>();
            MoqExtensions.ConfigureMock(dbSetMock, entities); // no findByKeyExpression

            // Act
            var result = await dbSetMock.Object.FindAsync(1);

            // Assert
            Assert.Null(result);
        }

        public class TestEntity
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
        }
    }
}
