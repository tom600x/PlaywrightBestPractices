using EmployeeDirectory.EngineeredTests.Models;
using EmployeeDirectory.EngineeredTests.Pages;
using EmployeeDirectory.EngineeredTests.Roles;
using EmployeeDirectory.EngineeredTests.Rules;
using Microsoft.Playwright;

namespace EmployeeDirectory.EngineeredTests.Workflows;

public sealed class EmployeeSearchWorkflow(EmployeeDirectoryPage directoryPage)
{
    public async Task SearchAndVerifyAsync(
        DirectoryActor actor,
        EmployeeTestData employee)
    {
        EmployeeBusinessRules.EnsureCanSearch(actor);

        await directoryPage.Search.SearchAsync(employee.Email);
        await Assertions.Expect(directoryPage.Employees.ResultCount)
            .ToHaveTextAsync("1 employee found");
        await Assertions.Expect(directoryPage.Employees.Table)
            .ToContainTextAsync(employee.FullName);
        await Assertions.Expect(directoryPage.Employees.Table)
            .ToContainTextAsync(employee.Email);
        await Assertions.Expect(directoryPage.Employees.Table)
            .ToContainTextAsync(employee.Title);
    }
}
