namespace EmployeeDirectory.Models;

public class EmployeeDetailsViewModel
{
    public required Employee Employee { get; init; }
    public IReadOnlyList<EmployeeStatusHistory> StatusHistory { get; init; } = [];
}
