namespace ProjectName.Data.Sqlite.Models;

public sealed record User(
	Guid Id,
	string Username,
	string Password,
	string Fullname,
	string Email,
	string Roles,
	bool IsLocked):BaseModel;
