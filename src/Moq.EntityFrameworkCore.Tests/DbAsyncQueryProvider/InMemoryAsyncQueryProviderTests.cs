namespace Moq.EntityFrameworkCore.Tests.DbAsyncQueryProvider
{
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using EntityFrameworkCore.DbAsyncQueryProvider;
    using Xunit;

    public class InMemoryAsyncQueryProviderTests
    {
        private readonly Mock<IQueryProvider> queryProviderMock = new Mock<IQueryProvider>();
        private readonly Expression expression = new Mock<Expression>().Object;
        private readonly InMemoryAsyncQueryProvider<int> inMemoryAsyncQueryProvider;

        public InMemoryAsyncQueryProviderTests()
        {
            this.inMemoryAsyncQueryProvider = new InMemoryAsyncQueryProvider<int>(this.queryProviderMock.Object);
        }

        [Fact]
        public void Given_InMemoryAsyncQueryProvider_When_CreatingQuery_Then_CorrectInMemoryAsyncEnumerableIsReturned()
        {
            // Act
            IQueryable result = this.inMemoryAsyncQueryProvider.CreateQuery(this.expression);

            // Assert
            Assert.IsType<InMemoryAsyncEnumerable<int>>(result);
            Assert.Equal(this.expression, result.Expression);
        }

        [Fact]
        public void Given_InMemoryAsyncQueryProvider_When_CreatingQueryGeneric_Then_CorrectInMemoryAsyncEnumerableIsReturned()
        {
            // Act
            IQueryable result = this.inMemoryAsyncQueryProvider.CreateQuery<int>(this.expression);

            // Assert
            Assert.IsType<InMemoryAsyncEnumerable<int>>(result);
            Assert.Equal(this.expression, result.Expression);
        }

        [Fact]
        public void Given_InMemoryAsyncQueryProvider_When_ExecutingQuery_Then_ExecutionIsDoneAtInnerQueryProvider()
        {
            // Act
            this.inMemoryAsyncQueryProvider.Execute(this.expression);

            // Assert
            this.queryProviderMock.Verify(x=> x.Execute(this.expression));
        }

        [Fact]
        public void Given_InMemoryAsyncQueryProvider_When_ExecutingQueryGeneric_Then_ExecutionIsDoneAtInnerQueryProvider()
        {
            // Act
            this.inMemoryAsyncQueryProvider.Execute<int>(this.expression);

            // Assert
            this.queryProviderMock.Verify(x => x.Execute<int>(this.expression));
        }

        [Fact]
        public void Given_InMemoryAsyncQueryProvider_When_ExecutingQueryAsync_Then_ExecutionIsDoneAtInnerQueryProvider()
        {
            // Act
            this.inMemoryAsyncQueryProvider.ExecuteAsync<int>(this.expression, CancellationToken.None);

            // Assert
            this.queryProviderMock.Verify(x => x.Execute<int>(this.expression));
        }

        [Fact]
        public void Given_InMemoryAsyncQueryProvider_When_ExecutingQueryAsyncWithCancellationToken_Then_ExecutionIsDoneAtInnerQueryProvider()
        {
            // Act
            this.inMemoryAsyncQueryProvider.ExecuteAsync(this.expression, CancellationToken.None);

            // Assert
            this.queryProviderMock.Verify(x => x.Execute(this.expression));
        }

        [Fact]
        public void Given_InMemoryAsyncQueryProvider_When_ExecutingQueryAsyncAndGeneric_Then_ExecutionIsDoneAtInnerQueryProvider()
        {
            // Act
            this.inMemoryAsyncQueryProvider.ExecuteAsync<int>(this.expression, CancellationToken.None);

            // Assert
            this.queryProviderMock.Verify(x => x.Execute<int>(this.expression));
        }
    }
}
