namespace ProjectName.API.Endpoints;
public sealed class UserEndpoints(
    ILogger<UserEndpoints> logger,
    INotifications notifications,
    HybridCache cache,
    IHttpContextAccessor httpContext,
    IServices<User, Guid> services) : IEndpoints
{
    private readonly HybridCacheEntryOptions hybridCacheEntryOptions = new()
    {
        Expiration = TimeSpan.FromMinutes(5),
        LocalCacheExpiration = TimeSpan.FromMinutes(5),
    };
    private const string version = "/api/v1/";
    private const string keyName = "Users";
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

    private async Task<IResult> Create(UserDto request, CancellationToken cancellationToken)
    {
        if (!request.IsValid)
        {
            var errors = request.Errors()?.ToDictionary(x => x.Key, x => new[] { x.Value })!;
            return Results.ValidationProblem(errors, "Validation problem", statusCode: 404);
        }

        var hasher = new PasswordHasher<object>();
        var passwordHash = hasher.HashPassword(null!, request.Password);
        request.Password = passwordHash;

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
            logger.LogWarning("User Id: {id} was accessed which doesn't exist.", id);
            return Results.NotFound("User not found.");
        }
        logger.LogInformation("User Id: {id} was accessed.", id);
        await notifications.Notify(result, $"{id}", NotificationAction.Read);
        return Results.Ok(result);
    }

    private async Task<IResult> GetAll(string? search = null, CancellationToken cancellationToken = default)
    {
        var pagedKey = $"{nameof(User)}-{search}";
        var result = await cache.GetOrCreateAsync(
            $"{pagedKey}",
            async ct => await ReadRecords(search, ct),
            hybridCacheEntryOptions,
            tags: [keyName],
            cancellationToken: cancellationToken
        );

        if (result?.Length == 0)
        {
            logger.LogWarning("All Users accessed but none exists.");
            return Results.NotFound("Users not found.");
        }
        logger.LogInformation("All Users accessed.");
        await notifications.Notify<User>(default, null, NotificationAction.Read);
        return Results.Ok(result);
    }

    private async Task<User[]?> ReadRecords(string? search, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(search)) return await services.ReadAll(null, ct);

        return await services.ReadAll((x =>
            x.Id.ToString().Match(search) ||
            x.Username.ToString().Match(search) ||
            x.Password.ToString().Match(search) ||
            x.Fullname.ToString().Match(search) ||
            x.Email.ToString().Match(search) ||
            x.Roles.ToString().Match(search) ||
            x.IsLocked.ToString().Match(search)
        ), ct);
    }

    private async Task<IResult> GetPaged(int size, int page, bool isAscending, string? property = null, string? search = null, CancellationToken cancellationToken = default)
    {
        var pagedKey = $"{nameof(User)}-{size}-{page}-{isAscending}-{property}-{search}";

        var result = await cache.GetOrCreateAsync(
            $"{pagedKey}",
            async ct => await ReadPagedRecords(size, page, isAscending, property, search, ct),
            hybridCacheEntryOptions,
            tags: [keyName],
            cancellationToken: cancellationToken
        );

        if (result?.Data?.Length == 0)
        {
            logger.LogWarning("Paged Users accessed but none exists.");
            return Results.NotFound("Users not found.");
        }
        logger.LogInformation("Paged Users accessed.");
        await notifications.Notify<PagedResult<User>>(default, null, NotificationAction.Read);
        return Results.Ok(result);
    }

    private async Task<PagedResult<User[]?>> ReadPagedRecords(int size, int page, bool isAscending, string? property, string? search, CancellationToken ct)
    {
        Func<User, dynamic?>? orderProperty = property switch
        {
            "Id" => x => x.Id,
            "Username" => x => x.Username,
            "Password" => x => x.Password,
            "Fullname" => x => x.Fullname,
            "Email" => x => x.Email,
            "Roles" => x => x.Roles,
            "IsLocked" => x => x.IsLocked,
            _ => null
        };

        if (string.IsNullOrWhiteSpace(search)) return await services.ReadAll(null, size, page, isAscending, orderProperty, ct);

        return await services.ReadAll((x =>
            x.Id.ToString().Match(search) ||
            x.Username.ToString().Match(search) ||
            x.Password.ToString().Match(search) ||
            x.Fullname.ToString().Match(search) ||
            x.Email.ToString().Match(search) ||
            x.Roles.ToString().Match(search) ||
            x.IsLocked.ToString().Match(search)
        ), size, page, isAscending, orderProperty, ct);
    }

    private async Task<IResult> Update(Guid id, UpdateUserDto request, CancellationToken cancellationToken)
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
            logger.LogWarning("Update failed for User Id: {id}", id);
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
            logger.LogWarning("Delete failed for User Id: {id}", id);
            return Results.BadRequest();
        }
        await cache.RemoveByTagAsync(keyName, cancellationToken);
        await notifications.Notify<User>(null, $"{id}", NotificationAction.Deleted);
        return Results.NoContent();
    }
}
