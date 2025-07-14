namespace ProjectName.UI.Services;

public static class CompositeExtensions
{
    private static readonly JsonSerializerOptions jso = new() { PropertyNameCaseInsensitive = true };

    public static string? ToJson<T>(this T? obj)
    => obj is null ? default : JsonSerializer.Serialize(obj, jso);

    public static T? FromJson<T>(this string? json)
        => string.IsNullOrWhiteSpace(json) ? default : JsonSerializer.Deserialize<T>(json, jso);

    public static string? Overflow(this string? value, int length = 25)
        => value?.Length > length ? $"{value[..length]}..." : value;

    public static T As<T>(this object source) => (T)source;
    public static T As<T>(this object[] source) => (T)source[0];
    public static (T1, T2) As<T1, T2>(this object[] source) => ((T1)source[0], (T2)source[1]);
    public static (T1, T2, T3) As<T1, T2, T3>(this object[] source) => ((T1)source[0], (T2)source[1], (T3)source[2]);
    public static (T1, T2, T3, T4) As<T1, T2, T3, T4>(this object[] source)
        => ((T1)source[0], (T2)source[1], (T3)source[2], (T4)source[3]);
    public static (T1, T2, T3, T4, T5) As<T1, T2, T3, T4, T5>(this object[] source)
        => ((T1)source[0], (T2)source[1], (T3)source[2], (T4)source[3], (T5)source[4]);
    public static (T1, T2, T3, T4, T5, T6) As<T1, T2, T3, T4, T5, T6>(this object[] source)
        => ((T1)source[0], (T2)source[1], (T3)source[2], (T4)source[3], (T5)source[4], (T6)source[5]);
    public static (T1, T2, T3, T4, T5, T6, T7) As<T1, T2, T3, T4, T5, T6, T7>(this object[] source)
        => ((T1)source[0], (T2)source[1], (T3)source[2], (T4)source[3], (T5)source[4], (T6)source[5], (T7)source[6]);
    public static (T1, T2, T3, T4, T5, T6, T7,T8) As<T1, T2, T3, T4, T5, T6, T7,T8>(this object[] source)
        => ((T1)source[0], (T2)source[1], (T3)source[2], (T4)source[3], (T5)source[4], (T6)source[5], (T7)source[6], (T8)source[7]);

    public static T1? JsonAs<T1>(this object[] source)
        => JsonSerializer.Deserialize<T1>(source[0].ToString()!);
    public static (T1?, T2?) JsonAs<T1, T2>(this object[] source)
        => (JsonSerializer.Deserialize<T1>(source[0].ToString()!),
            JsonSerializer.Deserialize<T2>(source[1].ToString()!));
    public static (T1?, T2?, T3?) JsonAs<T1, T2, T3>(this object[] source)
        => (JsonSerializer.Deserialize<T1>(source[0].ToString()!),
            JsonSerializer.Deserialize<T2>(source[1].ToString()!),
            JsonSerializer.Deserialize<T3>(source[2].ToString()!));
    public static (T1?, T2?, T3?, T4?) JsonAs<T1, T2, T3, T4>(this object[] source)
        => (JsonSerializer.Deserialize<T1>(source[0].ToString()!),
            JsonSerializer.Deserialize<T2>(source[1].ToString()!),
            JsonSerializer.Deserialize<T3>(source[2].ToString()!),
            JsonSerializer.Deserialize<T4>(source[3].ToString()!));
    public static (T1?, T2?, T3?, T4?, T5?) JsonAs<T1, T2, T3, T4, T5>(this object[] source)
        => (JsonSerializer.Deserialize<T1>(source[0].ToString()!),
            JsonSerializer.Deserialize<T2>(source[1].ToString()!),
            JsonSerializer.Deserialize<T3>(source[2].ToString()!),
            JsonSerializer.Deserialize<T4>(source[3].ToString()!),
            JsonSerializer.Deserialize<T5>(source[4].ToString()!));
    public static (T1?, T2?, T3?, T4?, T5?, T6?) JsonAs<T1, T2, T3, T4, T5, T6>(this object[] source)
        => (JsonSerializer.Deserialize<T1>(source[0].ToString()!),
            JsonSerializer.Deserialize<T2>(source[1].ToString()!),
            JsonSerializer.Deserialize<T3>(source[2].ToString()!),
            JsonSerializer.Deserialize<T4>(source[3].ToString()!),
            JsonSerializer.Deserialize<T5>(source[4].ToString()!),
            JsonSerializer.Deserialize<T6>(source[5].ToString()!));
    public static (T1?, T2?, T3?, T4?, T5?, T6?, T7?) JsonAs<T1, T2, T3, T4, T5, T6, T7>(this object[] source)
        => (JsonSerializer.Deserialize<T1>(source[0].ToString()!),
            JsonSerializer.Deserialize<T2>(source[1].ToString()!),
            JsonSerializer.Deserialize<T3>(source[2].ToString()!),
            JsonSerializer.Deserialize<T4>(source[3].ToString()!),
            JsonSerializer.Deserialize<T5>(source[4].ToString()!),
            JsonSerializer.Deserialize<T6>(source[5].ToString()!),
            JsonSerializer.Deserialize<T7>(source[6].ToString()!));

}