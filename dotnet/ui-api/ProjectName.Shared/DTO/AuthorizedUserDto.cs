namespace ProjectName.Shared.DTO;
public sealed class AuthorizedUserDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Fullname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string[] Roles { get; set; } = ["user"];
}
