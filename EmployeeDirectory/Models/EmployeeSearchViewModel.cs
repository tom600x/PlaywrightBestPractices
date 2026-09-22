namespace EmployeeDirectory.Models;

/// <summary>
/// View model backing the search/index page. Holds the current filter values
/// so the form can round-trip state and automation tools have stable fields to assert against.
/// </summary>
public class EmployeeSearchViewModel
{
    public string? SearchTerm { get; set; }
    public Department? Department { get; set; }
    public EmployeeStatus? Status { get; set; }

    public List<Employee> Results { get; set; } = new();

    public int TotalCount { get; set; }
}
