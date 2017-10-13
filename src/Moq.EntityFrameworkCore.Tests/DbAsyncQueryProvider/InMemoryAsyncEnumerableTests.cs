namespace Moq.EntityFrameworkCore.Tests.DbAsyncQueryProvider
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using EntityFrameworkCore.DbAsyncQueryProvider;
    using Xunit;

    public static class InMemoryAsyncEnumerableTests
    {
        [Fact]
        public static void Given_InMemoryAsyncEnumerable_When_GetAsyncEnumerator_Then_EnumeratorFromInInnerEnumerableShouldBeUsed()
        {
            // Arrange
            var enumerableMock = new Mock<IEnumerable<int>>();
            var inMemoryAsyncEnumerable = new InMemoryAsyncEnumerable<int>(enumerableMock.Object);

            // Act
            inMemoryAsyncEnumerable.GetAsyncEnumerator();

            // Assert
            enumerableMock.Verify(x => x.GetEnumerator());
        }

        [Fact]
        public static void Given_InMemoryAsyncEnumerable_When_GetEnumerator_Then_EnumeratorFromInInnerEnumerableShouldBeUsed()
        {
            // Arrange
            var enumerableMock = new Mock<IEnumerable<int>>();
            var inMemoryAsyncEnumerable = new InMemoryAsyncEnumerable<int>(enumerableMock.Object);

            // Act
            inMemoryAsyncEnumerable.GetEnumerator();

            // Assert
            enumerableMock.Verify(x => x.GetEnumerator());
        }

        [Fact]
        public static void Given_Enumerable_When_ObjectCreated_Then_ObjectCorrectlyBuild()
        {
            // Arrange
            var enumerableMock = new Mock<IEnumerable<int>>();

            // Act
            var inMemoryAsyncEnumerable = new InMemoryAsyncEnumerable<int>(enumerableMock.Object);

            // Assert
            Assert.Equal(enumerableMock.Object.GetEnumerator(), ((IEnumerable<int>)inMemoryAsyncEnumerable).GetEnumerator());
        }

        [Fact]
        public static void Given_Expression_When_ObjectCreated_Then_ObjectCorrectlyBuild()
        {
            // Arrange
            var expressionMock = new Mock<Expression>().Object;

            // Act
            var inMemoryAsyncEnumerable = new InMemoryAsyncEnumerable<int>(expressionMock);

            // Assert
            Assert.Equal(expressionMock, ((IQueryable)inMemoryAsyncEnumerable).Expression);
        }

        [Fact]
        public static void Given_Expression_When_ObjectCreated_Then_ProviderIsCorrect()
        {
            // Arrange
            var expressionMock = new Mock<Expression>().Object;
            IQueryable queryableProvider = new InMemoryAsyncEnumerable<int>(expressionMock);

            // Act
            var result = queryableProvider.Provider;

            // Assert
            Assert.NotNull(result);
        }
    }
}
