using Benchmark.Clients;
using Benchmark.Database;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Globalization;

namespace Benchmark;

[SimpleJob(RunStrategy.Monitoring, launchCount: 1, warmupCount: 0, iterationCount: 1, invocationCount: 1)]
[StopOnFirstError]
public class Benchmarks
{
    [Params(10_000, 1_000_000, 100_000_000)]
    public int Entries;

    [Params(10, 100)]
    public int ConcurrentUsers;

    [Params(0.1, 0.5, 0.9)]
    public double PercentageOfDeletes;

    private const int _numberOfCallsPerClient = 100;

    private static IDbContextFactory<TestDatabase> _contextFactory = new SimpleContextFactory(0);

    private static Dictionary<string, ApiResponseSummary> _apiResults = [];

    [IterationSetup]
    public void IterationSetup()
    {
        using var context = _contextFactory.CreateDbContext();

        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        var initialEntries = Enumerable.Range(0, Entries).Select(i => new TestEntity { SomeData = "foo" + i });
        context.TestEntities.AddRange(initialEntries);
        context.SaveChanges();
    }

    [IterationCleanup]
    public void IterationCleanup()
    {
        using var context = _contextFactory.CreateDbContext();

        context.Database.EnsureDeleted();
    }

    [GlobalCleanup]
    public void GlobalCleanup()
    {
        var writeHeader = !File.Exists("apiresults.md");
        using var stream = File.Open("apiresults.md", FileMode.Append);
        using var writer = new StreamWriter(stream);

        var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = writeHeader,
            Delimiter = " | "
        };
        using var csv = new CsvWriter(writer, csvConfig);

        var results = _apiResults
                .Select(p => new
                {
                    p.Key,
                    p.Value.NumberOfOkCallsOnGet,
                    p.Value.NumberOfNotFoundCallsOnGet,
                    p.Value.NumberOfOkCallsOnUpdate,
                    p.Value.NumberOfNotFoundCallsOnUpdate,
                    p.Value.NumberOfConflictCallsOnUpdate
                });

        csv.WriteRecords(results);

        csv.Flush();
        writer.Flush();
        stream.Flush();
    }

    [Benchmark(Baseline = true)]
    public async Task UpdateForceAsync()
    {
        var client = new ParallelClient();
        var results = await client.Run(ConcurrentUsers, _numberOfCallsPerClient, PercentageOfDeletes, (api, entity) => api.UpdateForceAsync(entity));
        _apiResults[nameof(UpdateForceAsync)] = results;
    }

    [Benchmark]
    public async Task UpdateForceReturnExistingAsync()
    {
        var client = new ParallelClient();
        var results = await client.Run(ConcurrentUsers, _numberOfCallsPerClient, PercentageOfDeletes, (api, entity) => api.UpdateForceReturnExistingAsync(entity));
        _apiResults[nameof(UpdateForceReturnExistingAsync)] = results;
    }

    [Benchmark]
    public async Task UpdateReQueryAsync()
    {
        var client = new ParallelClient();
        var results = await client.Run(ConcurrentUsers, _numberOfCallsPerClient, PercentageOfDeletes, (api, entity) => api.UpdateReQueryAsync(entity));
        _apiResults[nameof(UpdateReQueryAsync)] = results;
    }

    [Benchmark]
    public async Task UpdateReQueryReturnsExistingAsync()
    {
        var client = new ParallelClient();
        var results = await client.Run(ConcurrentUsers, _numberOfCallsPerClient, PercentageOfDeletes, (api, entity) => api.UpdateReQueryReturnsExistingAsync(entity));
        _apiResults[nameof(UpdateReQueryReturnsExistingAsync)] = results;
    }

    [Benchmark]
    public async Task UpdateReQuery2xAsync()
    {
        var client = new ParallelClient();
        var results = await client.Run(ConcurrentUsers, _numberOfCallsPerClient, PercentageOfDeletes, (api, entity) => api.UpdateReQuery2xAsync(entity));
        _apiResults[nameof(UpdateReQuery2xAsync)] = results;
    }

    [Benchmark]
    public async Task UpdateReQuery2xReturnExistingAsync()
    {
        var client = new ParallelClient();
        var results = await client.Run(ConcurrentUsers, _numberOfCallsPerClient, PercentageOfDeletes, (api, entity) => api.UpdateReQuery2xReturnExistingAsync(entity));
        _apiResults[nameof(UpdateReQuery2xReturnExistingAsync)] = results;
    }

    [Benchmark]
    public async Task UpdateRequeryLockAsync()
    {
        var client = new ParallelClient();
        var results = await client.Run(ConcurrentUsers, _numberOfCallsPerClient, PercentageOfDeletes, (api, entity) => api.UpdateRequeryLockAsync(entity));
        _apiResults[nameof(UpdateRequeryLockAsync)] = results;
    }
}
