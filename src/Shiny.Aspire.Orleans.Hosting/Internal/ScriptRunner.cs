using DbUp;
using DbUp.Builder;
using DbUp.Engine;
using Microsoft.Extensions.Logging;

namespace Shiny.Aspire.Orleans.Hosting.Internal;

internal static class ScriptRunner
{
    public static Task RunAsync(
        string connectionString,
        DatabaseType dbType,
        IEnumerable<SqlScriptSource> scripts,
        ILogger logger,
        CancellationToken cancellationToken = default)
    {
        var result = CreateConnection(dbType, connectionString)
            .WithVariablesDisabled()
            .WithScripts(scripts.Select((s, i) => new SqlScript(s.Name, s.Script)
            {
                SqlScriptOptions =
                {
                    RunGroupOrder = i
                }
            }))
            .LogTo(logger)
            .Build()
            .PerformUpgrade();

        if (!result.Successful)
        {
            return Task.FromException(result.Error ?? new Exception($"Error running script: {result.ErrorScript}"));
        }

        return Task.CompletedTask;
    }

    private static UpgradeEngineBuilder CreateConnection(DatabaseType dbType, string connectionString) => dbType switch
    {
        DatabaseType.SqlServer => DeployChanges.To.SqlDatabase(connectionString),
        DatabaseType.PostgreSQL => DeployChanges.To.PostgresqlDatabase(connectionString),
        DatabaseType.MySql => DeployChanges.To.MySqlDatabase(connectionString),
        _ => throw new ArgumentOutOfRangeException(nameof(dbType))
    };
}