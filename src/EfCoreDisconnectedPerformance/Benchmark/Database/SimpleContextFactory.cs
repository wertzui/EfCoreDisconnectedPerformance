using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;

namespace Benchmark.Database;

public class SimpleContextFactory(int millisecondsDelay) : IDbContextFactory<TestDatabase>
{
    private DbContextOptions<TestDatabase> _options = new DbContextOptionsBuilder<TestDatabase>().UseSqlServer("Data Source=(LocalDb)\\MSSQLLocalDB;Initial Catalog=EfCoreDisconnectedPerformance", o => o.EnableRetryOnFailure()).AddInterceptors(new Lag(millisecondsDelay)).Options;

    public TestDatabase CreateDbContext() => new(_options);

    private class Lag(int millisecondsDelay) : DbCommandInterceptor
    {
        public override async ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result, CancellationToken cancellationToken = default)
        {
            await Task.Delay(millisecondsDelay, cancellationToken);
            return await base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
        }
    }
}
