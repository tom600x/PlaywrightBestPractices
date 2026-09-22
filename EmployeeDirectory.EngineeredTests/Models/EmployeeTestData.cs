namespace EmployeeDirectory.EngineeredTests.Models;

public sealed record EmployeeTestData(
    string FirstName,
    string LastName,
    string Email,
    string Title,
    Department Department,
    EmployeeStatus Status,
    DateOnly HireDate)
{
    public string FullName => $"{FirstName} {LastName}";
}
