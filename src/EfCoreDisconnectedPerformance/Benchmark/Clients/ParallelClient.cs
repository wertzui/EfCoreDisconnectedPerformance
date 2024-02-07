using Benchmark.Api;
using Benchmark.Database;

namespace Benchmark.Clients;

public class ParallelClient
{
    public async Task<ApiResponseSummary> Run(int numberOfParallelClients, int numberOfCallsPerClient, double percentageOfDeletes, Func<Api.Api, TestEntity, Task<ApiResponse>> updateFunction)
    {
        var tasks = Enumerable.Range(0, numberOfParallelClients)
            .Select(i => Task.Run(() =>
            {
                var client = new Client();
                return client.Run(numberOfCallsPerClient, percentageOfDeletes, updateFunction);
            }))
            .ToList();

        var results = await Task.WhenAll(tasks);

        var aggregatedResults = results.Aggregate(new ApiResponseSummary(), (x, y) => x + y);

        return aggregatedResults;
    }
}
