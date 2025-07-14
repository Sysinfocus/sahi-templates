namespace ProjectName.Data.Sqlite.Models;

public sealed record RefreshToken(
	Guid Id,
	string UserId,
	string Token,
	DateTime ExpiresOn):BaseModel;
