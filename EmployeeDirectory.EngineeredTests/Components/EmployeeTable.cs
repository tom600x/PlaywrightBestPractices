using Microsoft.Playwright;

namespace EmployeeDirectory.EngineeredTests.Components;

public sealed class EmployeeTable(IPage page)
{
    public ILocator ResultCount => page.GetByTestId("result-count");
    public ILocator Table => page.GetByTestId("employee-table");

    public Task OpenDetailsByEmailAsync(string email)
    {
        var employeeRow = Table.Locator("tbody tr").Filter(new LocatorFilterOptions
        {
            HasText = email
        });
        return employeeRow.GetByRole(AriaRole.Link, new() { Name = "Details" }).ClickAsync();
    }
}
