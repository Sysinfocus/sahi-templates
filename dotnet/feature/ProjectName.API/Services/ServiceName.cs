public sealed class {{Model}}Services(AppDbContext context) : IServices<{{Model}}, {{IDType}}>
{
    public async Task<{{Model}}?> Create({{Model}} model, CancellationToken cancellationToken)
    {
        // If uniqueness is required, uncomment and validate accordingly.
        // var existing = await context.{{Table}}.FirstOrDefaultAsync(x => x.Name == model.Name, cancellationToken);
        // if (existing is not null) return default;
        context.{{Table}}.Add(model);
        await context.SaveChangesAsync(cancellationToken);
        return model;
    }

    public async Task<{{Model}}?> Read({{IDType}} id, CancellationToken cancellationToken)
        => await context.{{Table}}.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<{{Model}}[]?> ReadAll(Func<{{Model}}, bool>? predicate = null, CancellationToken cancellationToken = default)
    {
        var result = context.{{Table}}.AsNoTracking();
        if (predicate is not null) return Task.FromResult<{{Model}}[]?>([.. result.Where(predicate)]);
        return Task.FromResult<{{Model}}[]?>([.. result]);
    }

    public Task<PagedResult<{{Model}}[]?>> ReadAll<T>(Func<{{Model}}, bool>? predicate = null, int size = 0, int page = 0,
        bool isAscending = true, Func<{{Model}}, T>? order = null, CancellationToken cancellationToken = default)
    {
        var result = context.{{Table}}.AsNoTracking().OrderBy(x => x.Id).AsQueryable();
        if (predicate is not null) result = result.Where(predicate).AsQueryable();
        
        if (isAscending && order is not null) result = result.OrderBy(order).AsQueryable();
        else if (!isAscending && order is not null) result = result.OrderByDescending(order).AsQueryable();
        
        int totalRecords = result.Count();
        if (size != 0 && page == 0) result = result.Take(size);
        else if (size != 0 && page != 0) result = result.Skip(size * (page - 1)).Take(size);
        var final = result.ToArray();
        return Task.FromResult(new PagedResult<{{Model}}[]?>(final, size, page, totalRecords));
    }

    public async Task<bool> Update({{IDType}} id, {{Model}} model, CancellationToken cancellationToken)
    {
        var existing = await context.{{Table}}.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (existing is null) return false;
        context.ChangeTracker.Clear();
        var updated = model with { Id = existing.Id };
        context.{{Table}}.Update(updated);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> Delete({{IDType}} id, Guid deletedBy, CancellationToken cancellationToken)
    {
        var existing = await context.{{Table}}.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (existing is null) return false;
        context.ChangeTracker.Clear();
        var delete = existing with { Id = id };
        if (deletedBy == Guid.Empty)
            context.{{Table}}.Remove(delete);
        else
        {
            delete.IsDeleted = true;
            delete.ModifiedBy = deletedBy;
            delete.ModifiedOn = DateTime.UtcNow;
            context.{{Table}}.Update(delete);
        }
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}