using EmployeeDirectory.EngineeredTests.Models;
using EmployeeDirectory.EngineeredTests.Pages;
using EmployeeDirectory.EngineeredTests.Roles;
using EmployeeDirectory.EngineeredTests.Rules;
using Microsoft.Playwright;

namespace EmployeeDirectory.EngineeredTests.Workflows;

public sealed class EmployeeCreationWorkflow(
    EmployeeDirectoryPage directoryPage,
    CreateEmployeePage createEmployeePage)
{
    public async Task CreateAsync(
        DirectoryActor actor,
        EmployeeTestData employee)
    {
        EmployeeBusinessRules.EnsureCanCreate(actor, employee);

        await directoryPage.OpenCreateEmployeeAsync();
        await Assertions.Expect(createEmployeePage.PageTitle)
            .ToHaveTextAsync("Add Employee");

        await createEmployeePage.Form.FillAsync(employee);
        await createEmployeePage.Form.SaveAsync();

        await Assertions.Expect(directoryPage.StatusMessage)
            .ToContainTextAsync(
                $"Employee \"{employee.FullName}\" was created successfully.");
    }
}
