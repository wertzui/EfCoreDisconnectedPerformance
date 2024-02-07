using Benchmark.Database;
using System.Net;

namespace Benchmark.Api;

public record ApiResponse(HttpStatusCode StatusCode, TestEntity? Entity);
