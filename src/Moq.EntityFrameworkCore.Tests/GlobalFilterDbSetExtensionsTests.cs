namespace Moq.EntityFrameworkCore.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Xunit;

    public class GlobalFilterDbSetExtensionsTests
    {
        [Fact]
        public async Task ReturnsDbSetWithGlobalFilter_InterfaceSet_ReturnsOnlyMatchingEntities()
        {
            var active = new TestEntity { Id = 1, IsActive = true };
            var inactive = new TestEntity { Id = 2, IsActive = false };
            var context = new Mock<ITestContext>();
            context.Setup(c => c.Set<TestEntity>())
                .ReturnsDbSetWithGlobalFilter(new List<TestEntity> { active, inactive }, e => e.IsActive);

            Assert.Equal(new[] { active }, context.Object.Set<TestEntity>().ToList());
            Assert.Equal(new[] { active }, await context.Object.Set<TestEntity>().ToListAsync());
        }

        [Fact]
        public async Task ReturnsDbSetWithGlobalFilter_InterfacePropertyAndKey_FindsOnlyMatchingEntities()
        {
            var active = new TestEntity { Id = 1, IsActive = true };
            var inactive = new TestEntity { Id = 2, IsActive = false };
            var context = new Mock<ITestContext>();
            context.Setup(c => c.Entities)
                .ReturnsDbSetWithGlobalFilter(new List<TestEntity> { active, inactive }, e => e.IsActive, e => [e.Id]);

            Assert.Equal(new[] { active }, await context.Object.Entities.ToListAsync());
            Assert.Same(active, await context.Object.Entities.FindAsync(1));
            Assert.Null(await context.Object.Entities.FindAsync(2));
        }

        public interface ITestContext
        {
            DbSet<T> Set<T>() where T : class;
            DbSet<TestEntity> Entities { get; }
        }

        public class TestEntity
        {
            public int Id { get; set; }
            public bool IsActive { get; set; }
        }
    }
}
