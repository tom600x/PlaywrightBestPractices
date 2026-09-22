using EmployeeDirectory.EngineeredTests.Models;

namespace EmployeeDirectory.EngineeredTests.Data.Employees;

public static class SeededEmployees
{
    public static EmployeeTestData Engineer => new(
        "Ethan",
        "Brown",
        "ethan.brown@example.com",
        "QA Engineer",
        Department.Engineering,
        EmployeeStatus.Active,
        new DateOnly(2023, 2, 6));

    public static EmployeeTestData TerminatedSupportLead => new(
        "Sophia",
        "Rossi",
        "sophia.rossi@example.com",
        "Customer Support Lead",
        Department.Support,
        EmployeeStatus.Terminated,
        new DateOnly(2017, 9, 2));
}
