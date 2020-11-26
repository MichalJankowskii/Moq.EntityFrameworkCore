namespace Moq.EntityFrameworkCore.DbAsyncQueryProvider
{
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore.Query;

	public class InMemoryAsyncQueryProvider<TEntity> : IAsyncQueryProvider
    {
        private readonly IQueryProvider innerQueryProvider;

        public InMemoryAsyncQueryProvider(IQueryProvider innerQueryProvider)
        {
            this.innerQueryProvider = innerQueryProvider;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return new InMemoryAsyncEnumerable<TEntity>(expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new InMemoryAsyncEnumerable<TElement>(expression);
        }

        public object Execute(Expression expression)
        {
            return this.innerQueryProvider.Execute(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return this.innerQueryProvider.Execute<TResult>(expression);
        }

        public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = Execute(expression);
            
            var expectedResultType = typeof(TResult).GetGenericArguments()?.FirstOrDefault();
            if (expectedResultType == null)
            {
                return default(TResult);
            }

            return (TResult)typeof(Task).GetMethod(nameof(Task.FromResult))
                ?.MakeGenericMethod(expectedResultType)
                .Invoke(null, new[] { result });
        }


        public Task<object> ExecuteAsync(Expression expression, CancellationToken cancellationToken)
        {
            return Task.FromResult(this.Execute(expression));
        }
    }
}
