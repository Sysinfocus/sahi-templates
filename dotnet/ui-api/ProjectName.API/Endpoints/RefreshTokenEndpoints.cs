namespace ProjectName.API.Endpoints;
public sealed class RefreshTokenEndpoints(
    ILogger<RefreshTokenEndpoints> logger,
    INotifications notifications,
    HybridCache cache,
    IHttpContextAccessor httpContext,
    IServices<RefreshToken, Guid> services) : IEndpoints
{
    private readonly HybridCacheEntryOptions hybridCacheEntryOptions = new()
    {
        Expiration = TimeSpan.FromMinutes(5),
        LocalCacheExpiration = TimeSpan.FromMinutes(5),
    };
    private const string version = "/api/v1/";
    private const string keyName = "RefreshTokens";
    public void Register(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(version + keyName).RequireAuthorization();

        group.MapGet("/{id:Guid}", Get);
        group.MapGet("/{search?}", GetAll);
        group.MapGet("/{size:int}/{page:int}/{isAscending:bool}/{property?}/{search?}", GetPaged);
        group.MapPost("/", Create);
        group.MapPut("/{id:Guid}", Update);
        group.MapDelete("/{id:Guid}", Delete);
    }

    private async Task<IResult> Create(RefreshTokenDto request, CancellationToken cancellationToken)
    {
        if (!request.IsValid)
        {
            var errors = request.Errors()?.ToDictionary(x => x.Key, x => new[] { x.Value })!;
            return Results.ValidationProblem(errors, "Validation problem", statusCode: 404);
        }

        var model = request.ToModel();
        var result = await services.Create(model, cancellationToken);
        if (result is null)
        {
            var errors = new Dictionary<string, string[]>
            {
                // Add validations as required
            };
            logger.LogError("Validation error for request: {request}", request);
            return Results.ValidationProblem(errors, null, null, 400, "Validation error.");
        }
        await cache.RemoveByTagAsync(keyName, cancellationToken);
        await notifications.Notify(result, result.Id.ToString(), NotificationAction.Created);
        return Results.Created($"{version}{keyName}/{result.Id}", result.ToDTO());
    }

    private async Task<IResult> Get(Guid id, CancellationToken cancellationToken)
    {
        var result = await cache.GetOrCreateAsync(
            $"{keyName}-{id}",
            async ct => await services.Read(id, ct),
            hybridCacheEntryOptions,
            tags: [keyName],
            cancellationToken: cancellationToken
        );
        if (result is null)
        {
            await cache.RemoveAsync($"{keyName}-{id}", cancellationToken);
            logger.LogWarning("RefreshToken Id: {id} was accessed which doesn't exist.", id);
            return Results.NotFound("RefreshToken not found.");
        }
        logger.LogInformation("RefreshToken Id: {id} was accessed.", id);
        await notifications.Notify(result, $"{id}", NotificationAction.Read);
        return Results.Ok(result);
    }

    private async Task<IResult> GetAll(string? search = null, CancellationToken cancellationToken = default)
    {
        var pagedKey = $"{nameof(RefreshToken)}-{search}";
        var result = await cache.GetOrCreateAsync(
            $"{pagedKey}",
            async ct => await ReadRecords(search, ct),
            hybridCacheEntryOptions,
            tags: [keyName],
            cancellationToken: cancellationToken
        );

        if (result?.Length == 0)
        {
            logger.LogWarning("All RefreshTokens accessed but none exists.");
            return Results.NotFound("RefreshTokens not found.");
        }
        logger.LogInformation("All RefreshTokens accessed.");
        await notifications.Notify<RefreshToken>(default, null, NotificationAction.Read);
        return Results.Ok(result);
    }

    private async Task<RefreshToken[]?> ReadRecords(string? search, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(search)) return await services.ReadAll(null, ct);

        return await services.ReadAll((x =>
                        x.Id.ToString().Match(search) ||
            x.UserId.ToString().Match(search) ||
            x.Token.ToString().Match(search) ||
            x.ExpiresOn.ToString().Match(search)
        ), ct);
    }

    private async Task<IResult> GetPaged(int size, int page, bool isAscending, string? property = null, string? search = null, CancellationToken cancellationToken = default)
    {
        var pagedKey = $"{nameof(RefreshToken)}-{size}-{page}-{isAscending}-{property}-{search}";

        var result = await cache.GetOrCreateAsync(
            $"{pagedKey}",
            async ct => await ReadPagedRecords(size, page, isAscending, property, search, ct),
            hybridCacheEntryOptions,
            tags: [keyName],
            cancellationToken: cancellationToken
        );

        if (result?.Data?.Length == 0)
        {
            logger.LogWarning("Paged RefreshTokens accessed but none exists.");
            return Results.NotFound("RefreshTokens not found.");
        }
        logger.LogInformation("Paged RefreshTokens accessed.");
        await notifications.Notify<PagedResult<RefreshToken>>(default, null, NotificationAction.Read);
        return Results.Ok(result);
    }

    private async Task<PagedResult<RefreshToken[]?>> ReadPagedRecords(int size, int page, bool isAscending, string? property, string? search, CancellationToken ct)
    {
        Func<RefreshToken, dynamic?>? orderProperty = property switch
        {
            "Id" => x => x.Id,
            "UserId" => x => x.UserId,
            "Token" => x => x.Token,
            "ExpiresOn" => x => x.ExpiresOn,
            _ => null
        };

        if (string.IsNullOrWhiteSpace(search)) return await services.ReadAll(null, size, page, isAscending, orderProperty, ct);

        return await services.ReadAll((x =>
            x.Id.ToString().Match(search) ||
            x.UserId.ToString().Match(search) ||
            x.Token.ToString().Match(search) ||
            x.ExpiresOn.ToString().Match(search)
        ), size, page, isAscending, orderProperty, ct);
    }

    private async Task<IResult> Update(Guid id, RefreshTokenDto request, CancellationToken cancellationToken)
    {
        if (!request.IsValid)
        {
            var errors = request.Errors()?.ToDictionary(x => x.Key, x => new[] { x.Value })!;
            return Results.ValidationProblem(errors, "Validation problem", statusCode: 404);
        }
        var model = request.ToModel(id);
        model.ModifiedBy = httpContext.GetUserId();
        model.ModifiedOn = DateTime.UtcNow;
        var result = await services.Update(id, model, cancellationToken);
        if (!result)
        {
            logger.LogWarning("Update failed for RefreshToken Id: {id}", id);
            return Results.BadRequest();
        }
        await cache.RemoveByTagAsync(keyName, cancellationToken);
        await notifications.Notify(model, $"{id}", NotificationAction.Updated);
        return Results.Accepted();
    }

    private async Task<IResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await services.Delete(id, httpContext.GetUserId(), cancellationToken);
        if (!result)
        {
            logger.LogWarning("Delete failed for RefreshToken Id: {id}", id);
            return Results.BadRequest();
        }
        await cache.RemoveByTagAsync(keyName, cancellationToken);
        await notifications.Notify<RefreshToken>(null, $"{id}", NotificationAction.Deleted);
        return Results.NoContent();
    }
}
