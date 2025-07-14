namespace ProjectName.API.Endpoints;

public class CompositeEndpoints(ILogger<CompositeEndpoints> logger, INotifications notifications, HybridCache cache) : IEndpoints
{
    private readonly HybridCacheEntryOptions hybridCacheEntryOptions = new()
    {
        Expiration = TimeSpan.FromMinutes(5),
        LocalCacheExpiration = TimeSpan.FromMinutes(5),
    };

    private const string version = "/api/v1/";
    private const string keyName = "Composite";
    public void Register(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(version + keyName).RequireAuthorization();

        group.MapGet("/{items}", GetCompositeItems);
        group.MapGet("/sql/{query}", GetQueryResult);
    }

    private IResult GetQueryResult(string query, AppDbContext context, CancellationToken cancellationToken)
    {
        using var connection = context.Database.GetDbConnection();
        using var command = connection.CreateCommand();
        connection.Open();
        command.CommandText = query;
        var reader = command.ExecuteReader();
        if (reader.HasRows)
        {
            var sb = new StringBuilder();
            var o = new object[reader.FieldCount];
            sb.Append("<thead><tr>");
            for (int i = 0; i < reader.FieldCount; i++)
                sb.Append($"<th>{reader.GetName(i)}</th>");
            sb.AppendLine("</tr></thead><tbody>");

            while (reader.Read())
            {
                reader.GetValues(o);
                sb.AppendLine("<tr><td>" + string.Join("</td><td>", o) + "</td></tr>");
            }
            sb.AppendLine("</tbody>");

            connection.Close();
            return Results.Content(sb.ToString().Trim());
        }
        else
        {
            connection.Close();
            return Results.Content("Command execution complete");
        }
    }

    private async Task<IResult> GetCompositeItems(string items, AppDbContext context, CancellationToken cancellationToken)
    {
        var result = await cache.GetOrCreateAsync(
            items,
            async ct => await GetItems(items, context, ct),
            hybridCacheEntryOptions,
            tags: [keyName],
            cancellationToken: cancellationToken
        );
        if (result is null)
        {
            await cache.RemoveAsync(items, cancellationToken);
            logger.LogWarning("Composite: {items} was accessed which doesn't exist.", items);
            return Results.NotFound("Composite call not found.");
        }
        logger.LogInformation("Composite: {items} was accessed.", items);
        await notifications.Notify(result, $"{items}", NotificationAction.Read);
        return Results.Ok(result);
    }

    private static async Task<object> GetItems(string items, AppDbContext context, CancellationToken cancellationToken)
    {
        var keys = items.Split(',');
        List<object> output = [];
        if (keys.Contains("constants")) output.Add(await GetConstants(context, cancellationToken));        
        return output;
    }

    private static async Task<object> GetConstants(AppDbContext context, CancellationToken cancellationToken)
        => await context.Constants.OrderBy(a => a.Group).ThenBy(a => a.Order).Select(x => new { x.Id, x.Group, x.Key, x.Value }).ToArrayAsync(cancellationToken);
}
