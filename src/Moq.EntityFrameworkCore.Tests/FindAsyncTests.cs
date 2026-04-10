namespace Moq.EntityFrameworkCore.Tests
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Dynamic;
    using Xunit;

    public class FindAsyncTests
    {
        [Fact]
        public async Task FindAsync_WithMatchingKey_ReturnsCorrectEntity()
        {
            // Arrange
            var entities = new List<TestEntitySimpleKey>
            {
                new() { Id = 1, Name = "Alice" },
                new() { Id = 2, Name = "Bob" },
                new() { Id = 3, Name = "Charlie" }
            };

            var dbSetMock = new Mock<DbSet<TestEntitySimpleKey>>();
            MoqExtensions.ConfigureMock(dbSetMock, entities, findByKeyExpression: e => [e.Id]);

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
            var entities = new List<TestEntitySimpleKey>
            {
                new() { Id = 1, Name = "Alice" },
                new() { Id = 2, Name = "Bob" }
            };

            var dbSetMock = new Mock<DbSet<TestEntitySimpleKey>>();
            MoqExtensions.ConfigureMock(dbSetMock, entities, findByKeyExpression: e => [e.Id]);

            // Act
            var result = await dbSetMock.Object.FindAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task FindAsync_WithCancellationToken_ReturnsCorrectEntity()
        {
            // Arrange
            var entities = new List<TestEntitySimpleKey>
            {
                new() { Id = 1, Name = "Alice" },
                new() { Id = 2, Name = "Bob" },
                new() { Id = 3, Name = "Charlie" }
            };

            var dbSetMock = new Mock<DbSet<TestEntitySimpleKey>>();
            MoqExtensions.ConfigureMock(dbSetMock, entities, findByKeyExpression: e => [e.Id]);

            using var cts = new CancellationTokenSource();

            // Act
            var result = await dbSetMock.Object.FindAsync([3], cts.Token);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Charlie", result.Name);
        }

        [Fact]
        public async Task FindAsync_WithoutFindByKeyExpression_ReturnsDefault()
        {
            // Arrange
            var entities = new List<TestEntitySimpleKey>
            {
                new() { Id = 1, Name = "Alice" }
            };

            var dbSetMock = new Mock<DbSet<TestEntitySimpleKey>>();
            MoqExtensions.ConfigureMock(dbSetMock, entities); // no findByKeyExpression

            // Act
            var result = await dbSetMock.Object.FindAsync(1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task FindAsync_WithCompositeKey_ReturnsCorrectEntity()
        {
            // Arrange
            var entities = new List<TestEntityCompositeKey>
            {
                new() { CountryCode = "ES", CityId = 1, Name = "Madrid" },
                new() { CountryCode = "ES", CityId = 2, Name = "Barcelona" },
                new() { CountryCode = "FR", CityId = 1, Name = "Paris" }
            };

            var dbSetMock = new Mock<DbSet<TestEntityCompositeKey>>();
            MoqExtensions.ConfigureMock(dbSetMock, entities, findByKeyExpression: e => [e.CountryCode, e.CityId]);

            // Act
            var result = await dbSetMock.Object.FindAsync("FR", 1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Paris", result.Name);
        }

        [Fact]
        public async Task FindAsync_WithCompositeKey_KeyValuesInWrongOrder_ReturnsNull()
        {
            // Arrange
            var entities = new List<TestEntityCompositeKey>
            {
                new() { CountryCode = "FR", CityId = 1, Name = "Paris" }
            };

            var dbSetMock = new Mock<DbSet<TestEntityCompositeKey>>();
            MoqExtensions.ConfigureMock(dbSetMock, entities, findByKeyExpression: e => [e.CountryCode, e.CityId]);

            // Act — key values passed in reverse order: (1, "FR") instead of ("FR", 1).
            // The order must be consistent with the findByKeyExpression lambda (e => [e.CountryCode, e.CityId]).
            // This is analogous to EF Core's own FindAsync contract: when using a real DbContext,
            // key values must be supplied in the same order as the key properties are declared in the model.
            // Passing them out of order produces no match, which is the expected and correct result.
            var result = await dbSetMock.Object.FindAsync(1, "FR");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task FindAsync_WithCompositeKey_DoesNotMatchPartialKey()
        {
            // Arrange
            var entities = new List<TestEntityCompositeKey>
            {
                new() { CountryCode = "ES", CityId = 1, Name = "Madrid" },
                new() { CountryCode = "FR", CityId = 1, Name = "Paris" }
            };

            var dbSetMock = new Mock<DbSet<TestEntityCompositeKey>>();
            MoqExtensions.ConfigureMock(dbSetMock, entities, findByKeyExpression: e => [e.CountryCode, e.CityId]);

            // Act
            var result = await dbSetMock.Object.FindAsync("DE", 1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task FindAsync_Dynamic_WithoutFindByKeyExpression_ReturnsDefault()
        {
            // Arrange
            var entities = new List<TestEntitySimpleKey>
            {
                new() { Id = 1, Name = "Alice" }
            };

            var dbSetMock = new Mock<DbSet<TestEntitySimpleKey>>();
            MoqExtensionsDynamic.ConfigureMockDynamic(dbSetMock, entities); // no findByKeyExpression

            // Act
            var result = await dbSetMock.Object.FindAsync(1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task FindAsync_Dynamic_WithMatchingKey_ReturnsCorrectEntity()
        {
            // Arrange
            var entities = new List<TestEntitySimpleKey>
            {
                new() { Id = 1, Name = "Alice" },
                new() { Id = 2, Name = "Bob" }
            };

            var dbSetMock = new Mock<DbSet<TestEntitySimpleKey>>();
            MoqExtensionsDynamic.ConfigureMockDynamic(dbSetMock, entities, findByKeyExpression: e => [e.Id]);

            // Act
            var result = await dbSetMock.Object.FindAsync(2);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Bob", result.Name);
        }

        [Fact]
        public async Task FindAsync_Dynamic_WithCancellationToken_ReturnsCorrectEntity()
        {
            // Arrange
            var entities = new List<TestEntitySimpleKey>
            {
                new() { Id = 1, Name = "Alice" },
                new() { Id = 2, Name = "Bob" }
            };

            var dbSetMock = new Mock<DbSet<TestEntitySimpleKey>>();
            MoqExtensionsDynamic.ConfigureMockDynamic(dbSetMock, entities, findByKeyExpression: e => [e.Id]);

            using var cts = new CancellationTokenSource();

            // Act
            var result = await dbSetMock.Object.FindAsync([1], cts.Token);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Alice", result.Name);
        }

        [Fact]
        public async Task FindAsync_Dynamic_WithCompositeKey_ReturnsCorrectEntity()
        {
            // Arrange
            var entities = new List<TestEntityCompositeKey>
            {
                new() { CountryCode = "ES", CityId = 1, Name = "Madrid" },
                new() { CountryCode = "FR", CityId = 1, Name = "Paris" }
            };

            var dbSetMock = new Mock<DbSet<TestEntityCompositeKey>>();
            MoqExtensionsDynamic.ConfigureMockDynamic(dbSetMock, entities, findByKeyExpression: e => [e.CountryCode, e.CityId]);

            // Act
            var result = await dbSetMock.Object.FindAsync("ES", 1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Madrid", result.Name);
        }

        [Fact]
        public async Task FindAsync_Dynamic_WithCompositeKey_KeyValuesInWrongOrder_ReturnsNull()
        {
            // Arrange
            var entities = new List<TestEntityCompositeKey>
            {
                new() { CountryCode = "FR", CityId = 1, Name = "Paris" }
            };

            var dbSetMock = new Mock<DbSet<TestEntityCompositeKey>>();
            MoqExtensionsDynamic.ConfigureMockDynamic(dbSetMock, entities, findByKeyExpression: e => [e.CountryCode, e.CityId]);

            // Act
            var result = await dbSetMock.Object.FindAsync(1, "FR");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task FindAsync_Dynamic_WithCompositeKey_DoesNotMatchPartialKey()
        {
            // Arrange
            var entities = new List<TestEntityCompositeKey>
            {
                new() { CountryCode = "ES", CityId = 1, Name = "Madrid" },
                new() { CountryCode = "FR", CityId = 1, Name = "Paris" }
            };

            var dbSetMock = new Mock<DbSet<TestEntityCompositeKey>>();
            MoqExtensionsDynamic.ConfigureMockDynamic(dbSetMock, entities, findByKeyExpression: e => [e.CountryCode, e.CityId]);

            // Act
            var result = await dbSetMock.Object.FindAsync("DE", 1);

            // Assert
            Assert.Null(result);
        }

        public class TestEntitySimpleKey
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
        }

        public class TestEntityCompositeKey
        {
            public string CountryCode { get; set; } = string.Empty;
            public int CityId { get; set; }
            public string Name { get; set; } = string.Empty;
        }
    }
}
