namespace ProjectName.UI.Services;

public sealed class Composite(ApiService apiService)
{
    public static readonly string[] GENDERS = ["Male", "Female", "Transgender", "Non-binary", "Other"];
    public static readonly string[] BLOODGROUPS = ["A+", "A-", "B+", "B-", "O+", "O-", "AB+", "AB-"];
    private static readonly string[] URLS =
    [
        "/api/v1/Courses",
        "/api/v1/Classes",
    ];

    public async Task<object[]> LoadAPIData(params SelectType[] api)
    {        
        List<Task<string>> tasks = [];
        foreach (var ac in api) tasks.Add(apiService.Get(URLS[(int)ac]));
        var results = await Task.WhenAll(tasks);
        var output = new object[api.Length];
        for (int i = 0; i < api.Length; i++)
        {
            output[i] = api[i] switch
            {
                // SelectType.Courses => results[i].FromJson<CourseDto[]>()!.OrderBy(o => o.Name).ToArray(),
                // SelectType.Classes => results[i].FromJson<ClassDto[]>()!.OrderBy(o => o.Name).ToArray(),
                //SelectType.Countries => results[i].FromJson<Country[]>()!.OrderBy(o => o.Name.Common).ToArray(),
                //SelectType.Users => results[i].FromJson<User[]>()!.OrderBy(o => o.Name).ToArray(),
                //SelectType.Posts => results[i].FromJson<Post[]>()!.OrderBy(o => o.Title).ToArray(),
                _ => throw new ArgumentOutOfRangeException(nameof(api), $"Unsupported API type: {api[i]}")
            };
        }
        return [.. output];
    }

}