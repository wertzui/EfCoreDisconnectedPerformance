using Benchmark.Api;
using Benchmark.Database;

namespace Benchmark.Clients;

public class Client
{
    private static Random _random = new Random(42);
    public async Task<ApiResponseSummary> Run(int numberOfCalls, double percentageOfDeletes, Func<Api.Api, TestEntity, Task<ApiResponse>> updateFunction)
    {
        var numberOfOkCallsOnGet = 0;
        var numberOfOkCallsOnUpdate = 0;
        var numberOfNotFoundCallsOnGet = 0;
        var numberOfNotFoundCallsOnUpdate = 0;
        var numberOfConflictCallsOnUpdate = 0;

        var api = new Api.Api();

        for (var i = 0; i < numberOfCalls; i++)
        {
            var getResponse = await api.GetAsync(i);

            if (getResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                numberOfNotFoundCallsOnGet++;
                continue;
            }

            numberOfOkCallsOnGet++;
            var entity = getResponse.Entity;

            var shouldDelete = _random.NextDouble() < percentageOfDeletes;
            if (shouldDelete)
            {
                await api.DeleteAsync(entity);
                continue;
            }

            entity.SomeData += _random.Next();
            var updateResponse = await updateFunction(api, entity);

            switch (updateResponse.StatusCode)
            {
                case System.Net.HttpStatusCode.NotFound:
                    numberOfNotFoundCallsOnUpdate++;
                    break;
                case System.Net.HttpStatusCode.Conflict:
                    numberOfConflictCallsOnUpdate++;
                    break;
                case System.Net.HttpStatusCode.OK:
                    numberOfOkCallsOnUpdate++;
                    break;
                default:
                    throw new Exception("No other status code should be returned.");
            }
        }

        return new(numberOfOkCallsOnGet, numberOfOkCallsOnUpdate, numberOfNotFoundCallsOnGet, numberOfNotFoundCallsOnUpdate, numberOfConflictCallsOnUpdate);
    }
}