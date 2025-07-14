namespace ProjectName.Shared.DTO;
public sealed class ConstantDto : ModelValidator
{
    public Guid Id { get; set; }
    public int Order { get; set; } = 100;
    public string Group { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string? Value { get; set; }

    public override Dictionary<string, string>? Errors()
    {
        var errors = new Dictionary<string, string>();
        if (!Group.IsLength(1,20)) errors.TryAdd(nameof(Group), "Group is required.");
        if (!Key.IsLength(1,50)) errors.TryAdd(nameof(Key), "Key is required.");
        return errors.Count == 0 ? null : errors;
    }
}
