
namespace ProjectName.Shared.DTO;
public sealed class RefreshTokenDto : ModelValidator
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresOn { get; set; }

    public override Dictionary<string, string>? Errors()
    {
        return null;
    }
}
