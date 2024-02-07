using System.ComponentModel.DataAnnotations;

namespace Benchmark.Database;

public class TestEntity
{
    public long Id { get; set; }
    public string SomeData { get; set; }

    [Timestamp]
    public byte[] Timestamp { get; set; }
}
