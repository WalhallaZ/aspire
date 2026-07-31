using Shiny.Aspire.Orleans.Hosting;
using Shiny.Aspire.Orleans.Hosting.Internal;
using Shouldly;

namespace Shiny.Aspire.Orleans.Tests;

public class ScriptLoaderTests
{
    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.MySql)]
    public void LoadCombinedScript_AllFeatures_ReturnsNonEmptyScript(DatabaseType dbType)
    {
        var script = ScriptLoader.LoadCombinedScript(dbType, OrleansFeature.All);
        script.Count.ShouldBe(4);

        script[0].Name.ShouldBe(ScriptLoader.ResourceName(dbType, "Main"));
        script[1].Name.ShouldBe(ScriptLoader.ResourceName(dbType, "Clustering"));
        script[2].Name.ShouldBe(ScriptLoader.ResourceName(dbType, "Persistence"));
        script[3].Name.ShouldBe(ScriptLoader.ResourceName(dbType, "Reminders"));
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.MySql)]
    public void LoadCombinedScript_ClusteringOnly_ReturnsScript(DatabaseType dbType)
    {
        var script = ScriptLoader.LoadCombinedScript(dbType, OrleansFeature.Clustering);
        script.Count.ShouldBe(2);

        script[0].Name.ShouldBe(ScriptLoader.ResourceName(dbType, "Main"));
        script[1].Name.ShouldBe(ScriptLoader.ResourceName(dbType, "Clustering"));
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.MySql)]
    public void LoadCombinedScript_PersistenceOnly_ReturnsScript(DatabaseType dbType)
    {
        var script = ScriptLoader.LoadCombinedScript(dbType, OrleansFeature.Persistence);
        script.Count.ShouldBe(2);

        script[0].Name.ShouldBe(ScriptLoader.ResourceName(dbType, "Main"));
        script[1].Name.ShouldBe(ScriptLoader.ResourceName(dbType, "Persistence"));
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.MySql)]
    public void LoadCombinedScript_RemindersOnly_ReturnsScript(DatabaseType dbType)
    {
        var script = ScriptLoader.LoadCombinedScript(dbType, OrleansFeature.Reminders);
        script.Count.ShouldBe(2);

        script[0].Name.ShouldBe(ScriptLoader.ResourceName(dbType, "Main"));
        script[1].Name.ShouldBe(ScriptLoader.ResourceName(dbType, "Reminders"));
    }

    [Theory]
    [InlineData(DatabaseType.SqlServer)]
    [InlineData(DatabaseType.PostgreSQL)]
    [InlineData(DatabaseType.MySql)]
    public void LoadCombinedScript_AllFeatures_ContainsMoreThanSingleFeature(DatabaseType dbType)
    {
        var allScript = ScriptLoader.LoadCombinedScript(dbType, OrleansFeature.All);
        var clusteringOnly = ScriptLoader.LoadCombinedScript(dbType, OrleansFeature.Clustering);

        allScript.Count.ShouldBeGreaterThan(clusteringOnly.Count);
    }

    [Theory]
    [InlineData(OrleansFeature.Clustering | OrleansFeature.Persistence)]
    [InlineData(OrleansFeature.Clustering | OrleansFeature.Reminders)]
    [InlineData(OrleansFeature.Persistence | OrleansFeature.Reminders)]
    public void LoadCombinedScript_FeatureCombinations_ReturnsScript(OrleansFeature features)
    {
        var script = ScriptLoader.LoadCombinedScript(DatabaseType.PostgreSQL, features);
        script.Count.ShouldBe(3);

        script[0].Name.ShouldBe(ScriptLoader.ResourceName(DatabaseType.PostgreSQL, "Main"));
        // Would be hard as hell to write other cases
    }

    [Fact]
    public void LoadCombinedScript_InvalidDatabaseType_Throws()
    {
        Should.Throw<ArgumentOutOfRangeException>(() => ScriptLoader.LoadCombinedScript((DatabaseType)99, OrleansFeature.All));
    }
}