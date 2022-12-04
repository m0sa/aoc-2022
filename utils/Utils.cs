namespace utils;
public static class Utils
{
    public static IEnumerable<string> EmbeddedResourceLines<T>(this T instance) where T : AOCDay
    {
        var t = instance.GetType();
        var resourceName = $"{t.Namespace}.{t.Name}.txt";
        var allResources = t.Assembly.GetManifestResourceNames().ToHashSet();
        using var str = t.Assembly.GetManifestResourceStream(resourceName) ?? throw new ArgumentException(
            $"EmbeddedResource '{resourceName}' not found, candidates are: {string.Join(", ", t.Assembly.GetManifestResourceNames())}");
        using var rdr = new StreamReader(str);
        string? line;
        while((line = rdr.ReadLine()) != null)
        {
            yield return line;
        }
    }
}
