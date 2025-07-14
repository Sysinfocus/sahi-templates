namespace ProjectName.Shared.DTO;

public sealed class UserDto : ModelValidator
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Fullname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Roles { get; set; } = "user";
    public bool IsLocked { get; set; }

    public override Dictionary<string, string>? Errors()
    {
        var errors = new Dictionary<string, string>();

        if (!Username.IsLength(5, 20)) errors.TryAdd(nameof(Username), $"{nameof(Username)} must be between 5 and 20 chars.");
        if (!Password.IsLength(6, 20)) errors.TryAdd(nameof(Password), $"{nameof(Password)} must be between 6 and 20 chars.");
        if (!Fullname.IsLength(6, 50)) errors.TryAdd(nameof(Fullname), $"{nameof(Fullname)} must be between 6 and 50 chars.");
        if (!Email.IsEmail()) errors.TryAdd(nameof(Email), $"{nameof(Email)} must be valid address.");

        return errors.Count > 0 ? errors : null;
    }
}

public class UpdateUserDto : ModelValidator
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Fullname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Roles { get; set; } = "user";
    public bool IsLocked { get; set; }

    public override Dictionary<string, string>? Errors()
    {
        var errors = new Dictionary<string, string>();

        if (!Username.IsLength(5, 20)) errors.TryAdd(nameof(Username), $"{nameof(Username)} must be between 5 and 20 chars.");
        if (!Fullname.IsLength(6, 50)) errors.TryAdd(nameof(Fullname), $"{nameof(Fullname)} must be between 6 and 50 chars.");
        if (!Email.IsEmail()) errors.TryAdd(nameof(Email), $"{nameof(Email)} must be valid address.");

        return errors.Count > 0 ? errors : null;
    }
}