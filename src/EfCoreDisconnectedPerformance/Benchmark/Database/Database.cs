using Microsoft.EntityFrameworkCore;

namespace Benchmark.Database;

public class TestDatabase : DbContext
{
    public TestDatabase(DbContextOptions<TestDatabase> options) : base(options)
    {
    }

    public DbSet<TestEntity> TestEntities => Set<TestEntity>();
}
