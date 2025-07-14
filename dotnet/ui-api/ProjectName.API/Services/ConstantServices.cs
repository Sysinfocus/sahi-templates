namespace ProjectName.API.Services;
public sealed class ConstantServices(AppDbContext context) : IServices<Constant, Guid>
{
    public async Task<Constant?> Create(Constant model, CancellationToken cancellationToken)
    {
        // If uniqueness is required, uncomment and validate accordingly.
        // var existing = await context.Constants.FirstOrDefaultAsync(x => x.Name == model.Name, cancellationToken);
        // if (existing is not null) return default;
        context.Constants.Add(model);
        await context.SaveChangesAsync(cancellationToken);
        return model;
    }

    public async Task<Constant?> Read(Guid id, CancellationToken cancellationToken)
        => await context.Constants.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<Constant[]?> ReadAll(Func<Constant, bool>? predicate = null, CancellationToken cancellationToken = default)
    {
        var result = context.Constants.AsNoTracking();
        if (predicate is not null) return Task.FromResult<Constant[]?>([.. result.Where(predicate)]);
        return Task.FromResult<Constant[]?>([.. result]);
    }

    public Task<PagedResult<Constant[]?>> ReadAll<T>(Func<Constant, bool>? predicate = null, int size = 0, int page = 0,
        bool isAscending = true, Func<Constant, T>? order = null, CancellationToken cancellationToken = default)
    {
        var result = context.Constants.AsNoTracking().OrderBy(x => x.Id).AsQueryable();
        if (predicate is not null) result = result.Where(predicate).AsQueryable();
        
        if (isAscending && order is not null) result = result.OrderBy(order).AsQueryable();
        else if (!isAscending && order is not null) result = result.OrderByDescending(order).AsQueryable();
        
        int totalRecords = result.Count();
        if (size != 0 && page == 0) result = result.Take(size);
        else if (size != 0 && page != 0) result = result.Skip(size * (page - 1)).Take(size);
        var final = result.ToArray();
        return Task.FromResult(new PagedResult<Constant[]?>(final, size, page, totalRecords));
    }

    public async Task<bool> Update(Guid id, Constant model, CancellationToken cancellationToken)
    {
        var existing = await context.Constants.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (existing is null) return false;
        context.ChangeTracker.Clear();
        var updated = model with { Id = existing.Id };
        context.Constants.Update(updated);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> Delete(Guid id, Guid deletedBy, CancellationToken cancellationToken)
    {
        var existing = await context.Constants.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (existing is null) return false;
        context.ChangeTracker.Clear();
        var delete = existing with { Id = id };
        if (deletedBy == Guid.Empty)
            context.Constants.Remove(delete);
        else
        {
            delete.IsDeleted = true;
            delete.ModifiedBy = deletedBy;
            delete.ModifiedOn = DateTime.UtcNow;
            context.Constants.Update(delete);
        }
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
