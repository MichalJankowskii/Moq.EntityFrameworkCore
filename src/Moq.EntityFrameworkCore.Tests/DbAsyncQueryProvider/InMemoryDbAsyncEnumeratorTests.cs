using System.Threading.Tasks;

namespace Moq.EntityFrameworkCore.Tests.DbAsyncQueryProvider
{
    using System.Collections.Generic;
    using System.Threading;
    using EntityFrameworkCore.DbAsyncQueryProvider;
    using Xunit;

    public class InMemoryDbAsyncEnumeratorTests
    {
        private readonly Mock<IEnumerator<int>> enumeratorMock = new Mock<IEnumerator<int>>();
        private readonly InMemoryDbAsyncEnumerator<int> inMemoryDbAsyncEnumerator;

        public InMemoryDbAsyncEnumeratorTests()
        {
            this.inMemoryDbAsyncEnumerator = new InMemoryDbAsyncEnumerator<int>(this.enumeratorMock.Object);
        }

        [Fact]
        public void Given_InMemoryDbAsyncEnumerator_When_Dispose_Then_InnerEnumeratorShouldBeDisposed()
        {
            // Act
            this.inMemoryDbAsyncEnumerator.Dispose();

            // Assert
            this.enumeratorMock.Verify(x => x.Dispose());
        }

        [Fact]
        public void Given_InMemoryDbAsyncEnumerator_When_Current_Then_CurrentFromInInnerEnumeratorShouldBeUsed()
        {
            // Act
            int result = this.inMemoryDbAsyncEnumerator.Current;

            // Assert
            this.enumeratorMock.VerifyGet(x=> x.Current);
        }

        [Fact]
        public async Task Given_InMemoryDbAsyncEnumerator_When_Current_Then_CurrentShouldBeSameAsInInnerEnumerator()
        {
            // Act
            await this.inMemoryDbAsyncEnumerator.MoveNext(CancellationToken.None);

            // Assert
            this.enumeratorMock.Verify(x => x.MoveNext());
        }
    }
}
