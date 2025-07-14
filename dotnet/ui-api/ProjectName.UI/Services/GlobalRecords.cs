namespace ProjectName.UI.Services;

public record Country(CountryName Name);
public record CountryName(string Common);
public record User(int Id, string Name);
public record Album(int UserId, int Id, string Title);
public record Post(int UserId, int Id, string Title, string Body);