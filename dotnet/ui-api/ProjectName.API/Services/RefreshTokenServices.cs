namespace ProjectName.API.Services;
public sealed class RefreshTokenServices(AppDbContext context) : IServices<RefreshToken, Guid>
{
    public async Task<RefreshToken?> Create(RefreshToken model, CancellationToken cancellationToken)
    {
        // If uniqueness is required, uncomment and validate accordingly.
        // var existing = await context.RefreshTokens.FirstOrDefaultAsync(x => x.Name == model.Name, cancellationToken);
        // if (existing is not null) return default;
        context.RefreshTokens.Add(model);
        await context.SaveChangesAsync(cancellationToken);
        return model;
    }

    public async Task<RefreshToken?> Read(Guid id, CancellationToken cancellationToken)
        => await context.RefreshTokens.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<RefreshToken[]?> ReadAll(Func<RefreshToken, bool>? predicate = null, CancellationToken cancellationToken = default)
    {
        var result = context.RefreshTokens.AsNoTracking();
        if (predicate is not null) return Task.FromResult<RefreshToken[]?>([.. result.Where(predicate)]);
        return Task.FromResult<RefreshToken[]?>([.. result]);
    }

    public Task<PagedResult<RefreshToken[]?>> ReadAll<T>(Func<RefreshToken, bool>? predicate = null, int size = 0, int page = 0,
        bool isAscending = true, Func<RefreshToken, T>? order = null, CancellationToken cancellationToken = default)
    {
        var result = context.RefreshTokens.AsNoTracking().OrderBy(x => x.Id).AsQueryable();
        if (predicate is not null) result = result.Where(predicate).AsQueryable();
        
        if (isAscending && order is not null) result = result.OrderBy(order).AsQueryable();
        else if (!isAscending && order is not null) result = result.OrderByDescending(order).AsQueryable();
        
        int totalRecords = result.Count();
        if (size != 0 && page == 0) result = result.Take(size);
        else if (size != 0 && page != 0) result = result.Skip(size * (page - 1)).Take(size);
        var final = result.ToArray();
        return Task.FromResult(new PagedResult<RefreshToken[]?>(final, size, page, totalRecords));
    }

    public async Task<bool> Update(Guid id, RefreshToken model, CancellationToken cancellationToken)
    {
        var existing = await context.RefreshTokens.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (existing is null) return false;
        context.ChangeTracker.Clear();
        var updated = model with { Id = existing.Id };
        context.RefreshTokens.Update(updated);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> Delete(Guid id, Guid deletedBy, CancellationToken cancellationToken)
    {
        var existing = await context.RefreshTokens.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (existing is null) return false;
        context.ChangeTracker.Clear();
        var delete = existing with { Id = id };
        if (deletedBy == Guid.Empty)
            context.RefreshTokens.Remove(delete);
        else
        {
            delete.IsDeleted = true;
            delete.ModifiedBy = deletedBy;
            delete.ModifiedOn = DateTime.UtcNow;
            context.RefreshTokens.Update(delete);
        }
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
