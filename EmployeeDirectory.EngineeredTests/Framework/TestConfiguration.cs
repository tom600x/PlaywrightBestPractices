namespace EmployeeDirectory.EngineeredTests.Framework;

public static class TestConfiguration
{
    public const string BaseUrl = "http://localhost:5080";

    public static string ProjectDirectory =>
        Directory.GetParent(AppContext.BaseDirectory)!.Parent!.Parent!.Parent!.FullName;

    public static string ReportsDirectory =>
        Path.Combine(ProjectDirectory, "reports");
}
