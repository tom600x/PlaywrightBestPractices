using EmployeeDirectory.EngineeredTests.Models;

namespace EmployeeDirectory.EngineeredTests.Data.Employees;

public static class EmployeeCreationData
{
    public static EmployeeTestData AutomationEngineer => new(
        "Jordan",
        "Lee",
        "jordan.lee.automation@example.com",
        "Automation Test Engineer",
        Department.Engineering,
        EmployeeStatus.Active,
        new DateOnly(2026, 9, 21));
}
