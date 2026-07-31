using System.Reflection;

namespace Shiny.Aspire.Orleans.Hosting.Internal;

internal static class ScriptLoader
{
    private static readonly Assembly Assembly = typeof(ScriptLoader).Assembly;

    public static IReadOnlyList<SqlScriptSource> LoadCombinedScript(DatabaseType dbType, OrleansFeature features)
    {
        var scripts = new List<SqlScriptSource>();

        var mainScript = LoadScript(dbType, "Main");
        if (mainScript != null)
            scripts.Add(mainScript.Value);

        if (features.HasFlag(OrleansFeature.Clustering))
        {
            var script = LoadScript(dbType, "Clustering");
            if (script != null)
                scripts.Add(script.Value);
        }

        if (features.HasFlag(OrleansFeature.Persistence))
        {
            var script = LoadScript(dbType, "Persistence");
            if (script != null)
                scripts.Add(script.Value);
        }

        if (features.HasFlag(OrleansFeature.Reminders))
        {
            var script = LoadScript(dbType, "Reminders");
            if (script != null)
                scripts.Add(script.Value);
        }

        return scripts;
    }

    internal static string ResourceName(DatabaseType dbType, string scriptName)
    {
        var dbFolder = dbType switch
        {
            DatabaseType.SqlServer => "SqlServer",
            DatabaseType.PostgreSQL => "PostgreSQL",
            DatabaseType.MySql => "MySql",
            _ => throw new ArgumentOutOfRangeException(nameof(dbType))
        };

        return $"Shiny.Aspire.Orleans.Hosting.Scripts.{dbFolder}.{scriptName}.sql";
    }

    private static SqlScriptSource? LoadScript(DatabaseType dbType, string scriptName)
    {
        var resourceName = ResourceName(dbType, scriptName);
        using var stream = Assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
            return null;

        using var reader = new StreamReader(stream);
        return new SqlScriptSource(resourceName, reader.ReadToEnd());
    }
}