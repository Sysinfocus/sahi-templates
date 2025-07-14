namespace ProjectName.Data.Sqlite.Models;

public sealed record Constant(
	Guid Id,
	int Order,
	string Group,
	string Key,
	string? Value):BaseModel;
