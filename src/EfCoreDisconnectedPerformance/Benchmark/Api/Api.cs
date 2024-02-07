using Benchmark.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;
using System.Net;

namespace Benchmark.Api;

public class Api
{
    private static IDbContextFactory<TestDatabase> _contextFactory = new SimpleContextFactory(10);

    public async Task<ApiResponse> GetAsync(long id)
    {
        using var context = _contextFactory.CreateDbContext();

        var entity = await context.TestEntities.AsNoTracking().FirstOrDefaultAsync(i => i.Id == id);
        var status = entity is null ? HttpStatusCode.NotFound : HttpStatusCode.OK;

        return new(status, entity);
    }

    public async Task<ApiResponse> UpdateForceAsync(TestEntity updatedEntity)
    {
        using var context = _contextFactory.CreateDbContext();

        context.TestEntities.Update(updatedEntity);

        try
        {
            await context.SaveChangesAsync();
            return new(HttpStatusCode.OK, updatedEntity);
        }
        catch (DbUpdateConcurrencyException)
        {
            return new(HttpStatusCode.Conflict, updatedEntity);
        }
    }

    public async Task<ApiResponse> UpdateForceReturnExistingAsync(TestEntity updatedEntity)
    {
        using var context = _contextFactory.CreateDbContext();

        context.TestEntities.Update(updatedEntity);

        try
        {
            await context.SaveChangesAsync();
            return new(HttpStatusCode.OK, updatedEntity);
        }
        catch (DbUpdateConcurrencyException)
        {
            var existingEntity = await context.TestEntities.AsNoTracking().FirstOrDefaultAsync(i => i.Id == updatedEntity.Id);
            var status = existingEntity is null ? HttpStatusCode.NotFound : HttpStatusCode.Conflict;

            return new(status, existingEntity);
        }
    }

    public async Task<ApiResponse> UpdateReQueryAsync(TestEntity updatedEntity)
    {
        using var context = _contextFactory.CreateDbContext();

        var existingEntity = await context.TestEntities.FirstOrDefaultAsync(i => i.Id == updatedEntity.Id);
        if (existingEntity is null)
            return new(HttpStatusCode.NotFound, updatedEntity);

        if (!existingEntity.Timestamp.SequenceEqual(updatedEntity.Timestamp))
            return new(HttpStatusCode.Conflict, existingEntity);

        existingEntity.SomeData = updatedEntity.SomeData;

        try
        {
            await context.SaveChangesAsync();
            return new(HttpStatusCode.OK, updatedEntity);
        }
        catch (DbUpdateConcurrencyException)
        {
            return new(HttpStatusCode.Conflict, updatedEntity);
        }
    }

    public async Task<ApiResponse> UpdateReQueryReturnsExistingAsync(TestEntity updatedEntity)
    {
        using var context = _contextFactory.CreateDbContext();

        var existingEntity = await context.TestEntities.FirstOrDefaultAsync(i => i.Id == updatedEntity.Id);
        if (existingEntity is null)
            return new(HttpStatusCode.NotFound, updatedEntity);

        if (!existingEntity.Timestamp.SequenceEqual(updatedEntity.Timestamp))
            return new(HttpStatusCode.Conflict, existingEntity);

        existingEntity.SomeData = updatedEntity.SomeData;

        try
        {
            await context.SaveChangesAsync();
            return new(HttpStatusCode.OK, updatedEntity);
        }
        catch (DbUpdateConcurrencyException)
        {
            var entityExists = await context.TestEntities.AnyAsync(i => i.Id == updatedEntity.Id);
            var status = entityExists ? HttpStatusCode.Conflict : HttpStatusCode.NotFound;

            return new(status, existingEntity);
        }
    }

    public async Task<ApiResponse> UpdateReQuery2xAsync(TestEntity updatedEntity)
    {
        using var context = _contextFactory.CreateDbContext();

        var existingEntity = await context.TestEntities.FirstOrDefaultAsync(i => i.Id == updatedEntity.Id);
        if (existingEntity is null)
            return new(HttpStatusCode.NotFound, updatedEntity);

        if (!existingEntity.Timestamp.SequenceEqual(updatedEntity.Timestamp))
            return new(HttpStatusCode.Conflict, existingEntity);

        existingEntity.SomeData = updatedEntity.SomeData;

        try
        {
            await context.SaveChangesAsync();
            return new(HttpStatusCode.OK, updatedEntity);
        }
        catch (DbUpdateConcurrencyException)
        {
            var entityExists = await context.TestEntities.AnyAsync(i => i.Id == updatedEntity.Id);
            var status = entityExists ? HttpStatusCode.Conflict : HttpStatusCode.NotFound;

            return new(status, existingEntity);
        }
    }

    public async Task<ApiResponse> UpdateReQuery2xReturnExistingAsync(TestEntity updatedEntity)
    {
        using var context = _contextFactory.CreateDbContext();

        var existingEntity = await context.TestEntities.FirstOrDefaultAsync(i => i.Id == updatedEntity.Id);
        if (existingEntity is null)
            return new(HttpStatusCode.NotFound, updatedEntity);

        if (!existingEntity.Timestamp.SequenceEqual(updatedEntity.Timestamp))
            return new(HttpStatusCode.Conflict, existingEntity);

        existingEntity.SomeData = updatedEntity.SomeData;

        try
        {
            await context.SaveChangesAsync();
            return new(HttpStatusCode.OK, updatedEntity);
        }
        catch (DbUpdateConcurrencyException)
        {
            existingEntity = await context.TestEntities.AsNoTracking().FirstOrDefaultAsync(i => i.Id == updatedEntity.Id);
            var status = existingEntity is null ? HttpStatusCode.NotFound : HttpStatusCode.Conflict;

            return new(status, existingEntity);
        }
    }

    public Task<ApiResponse> UpdateRequeryLockAsync(TestEntity updatedEntity)
    {
        using var context = _contextFactory.CreateDbContext();

        return context.Database.CreateExecutionStrategy().ExecuteInTransactionAsync<ApiResponse>(async t =>
        {
            var existingEntity = await context.TestEntities.FirstOrDefaultAsync(i => i.Id == updatedEntity.Id);
            if (existingEntity is null)
                return new(HttpStatusCode.NotFound, updatedEntity);

            if (!existingEntity.Timestamp.SequenceEqual(updatedEntity.Timestamp))
                return new(HttpStatusCode.Conflict, existingEntity);

            existingEntity.SomeData = updatedEntity.SomeData;

            await context.SaveChangesAsync();

            return new(HttpStatusCode.OK, updatedEntity);
        },
        t => Task.FromResult(false),
        IsolationLevel.Serializable,
        CancellationToken.None);
    }

    public async Task DeleteAsync(TestEntity entityToBeDeleted)
    {
        using var context = _contextFactory.CreateDbContext();

        context.Remove(entityToBeDeleted);

        // We do not want to test different kinds of delete, because the outcome would be similar to the update tests.
        try
        {
            await context.SaveChangesAsync();
        }
        catch (Exception) { }
    }
}
