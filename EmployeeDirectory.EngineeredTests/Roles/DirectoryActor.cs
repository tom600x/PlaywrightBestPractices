namespace EmployeeDirectory.EngineeredTests.Roles;

public sealed record DirectoryActor(
    string Name,
    bool CanSearchEmployees,
    bool CanCreateEmployees,
    bool CanChangeEmploymentStatus);

public static class TestRoles
{
    public static DirectoryActor DirectoryManager => new(
        "Directory Manager",
        CanSearchEmployees: true,
        CanCreateEmployees: true,
        CanChangeEmploymentStatus: true);
}
