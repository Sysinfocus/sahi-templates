public sealed class {{Model}}Endpoints(
    ILogger<{{Model}}Endpoints> logger,
    INotifications notifications,
    HybridCache cache,
    IHttpContextAccessor httpContext,
    IServices<{{Model}},{{IDType}}> services) : IEndpoints
{
    private readonly HybridCacheEntryOptions hybridCacheEntryOptions = new()
    {
        Expiration = TimeSpan.FromMinutes(5),
        LocalCacheExpiration = TimeSpan.FromMinutes(5),
    };
    private const string version = "/api/v1/";
    private const string keyName = "{{Table}}";
    public void Register(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(version + keyName).RequireAuthorization();

        group.MapGet("/{id:{{IDType}}}", Get);
        group.MapGet("/{search?}", GetAll);
        group.MapGet("/{size:int}/{page:int}/{isAscending:bool}/{property?}/{search?}", GetPaged);
        group.MapPost("/", Create);
        group.MapPut("/{id:{{IDType}}}", Update);
        group.MapDelete("/{id:{{IDType}}}", Delete);
    }

    private async Task<IResult> Create({{Model}}Dto request, CancellationToken cancellationToken)
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

    private async Task<IResult> Get({{IDType}} id, CancellationToken cancellationToken)
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
            logger.LogWarning("{{Model}} Id: {id} was accessed which doesn't exist.", id);
            return Results.NotFound("{{Model}} not found.");
        }
        logger.LogInformation("{{Model}} Id: {id} was accessed.", id);
        await notifications.Notify(result, $"{id}", NotificationAction.Read);
        return Results.Ok(result);
    }

    private async Task<IResult> GetAll(string? search = null, CancellationToken cancellationToken = default)
    {
        var pagedKey = $"{nameof({{Model}})}-{search}";
        var result = await cache.GetOrCreateAsync(
            $"{pagedKey}",
            async ct => await ReadRecords(search, ct),
            hybridCacheEntryOptions,
            tags: [keyName],
            cancellationToken: cancellationToken
        );

        if (result?.Length == 0)
        {
            logger.LogWarning("All {{Table}} accessed but none exists.");
            return Results.NotFound("{{Table}} not found.");
        }
        logger.LogInformation("All {{Table}} accessed.");
        await notifications.Notify<{{Model}}>(default, null, NotificationAction.Read);
        return Results.Ok(result);
    }
    
    private async Task<{{Model}}[]?> ReadRecords(string? search, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(search)) return await services.ReadAll(null, ct);
        
        return await services.ReadAll((x =>
            {{SearchPropertiesMapping}}
        ), ct);
    }

    private async Task<IResult> GetPaged(int size, int page, bool isAscending, string? property = null, string? search = null, CancellationToken cancellationToken = default)
    {
        var pagedKey = $"{nameof({{Model}})}-{size}-{page}-{isAscending}-{property}-{search}";

        var result = await cache.GetOrCreateAsync(
            $"{pagedKey}",
            async ct => await ReadPagedRecords(size, page, isAscending, property, search, ct),
            hybridCacheEntryOptions,
            tags: [keyName],
            cancellationToken: cancellationToken
        );

        if (result?.Data?.Length == 0)
        {
            logger.LogWarning("Paged {{Table}} accessed but none exists.");
            return Results.NotFound("{{Table}} not found.");
        }
        logger.LogInformation("Paged {{Table}} accessed.");
        await notifications.Notify<PagedResult<{{Model}}>>(default, null, NotificationAction.Read);
        return Results.Ok(result);
    }    

    private async Task<PagedResult<{{Model}}[]?>> ReadPagedRecords(int size, int page, bool isAscending, string? property, string? search, CancellationToken ct)
    {
        Func<{{Model}}, dynamic?>? orderProperty = property switch
        {
            {{SortPropertiesMapping}}
            ,_ => null
        };

        if (string.IsNullOrWhiteSpace(search)) return await services.ReadAll(null, size, page, isAscending, orderProperty, ct);

        return await services.ReadAll((x =>
            {{SearchPropertiesMapping}}
        ), size, page, isAscending, orderProperty, ct);
    }

    private async Task<IResult> Update({{IDType}} id, {{Model}}Dto request, CancellationToken cancellationToken)
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
            logger.LogWarning("Update failed for {{Model}} Id: {id}", id);
            return Results.BadRequest();
        }
        await cache.RemoveByTagAsync(keyName, cancellationToken);
        await notifications.Notify(model, $"{id}", NotificationAction.Updated);
        return Results.Accepted();
    }

    private async Task<IResult> Delete({{IDType}} id, CancellationToken cancellationToken)
    {
        var result = await services.Delete(id, httpContext.GetUserId(), cancellationToken);
        if (!result)
        {
            logger.LogWarning("Delete failed for {{Model}} Id: {id}", id);
            return Results.BadRequest();
        }
        await cache.RemoveByTagAsync(keyName, cancellationToken);
        await notifications.Notify<{{Model}}>(null, $"{id}", NotificationAction.Deleted);
        return Results.NoContent();
    }
}