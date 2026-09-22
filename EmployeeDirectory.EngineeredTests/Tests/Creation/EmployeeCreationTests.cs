using EmployeeDirectory.EngineeredTests.Data.Employees;
using EmployeeDirectory.EngineeredTests.Framework;
using EmployeeDirectory.EngineeredTests.Pages;
using EmployeeDirectory.EngineeredTests.Roles;
using EmployeeDirectory.EngineeredTests.Workflows;

namespace EmployeeDirectory.EngineeredTests.Tests.Creation;

public sealed class EmployeeCreationTests : EngineeredPageTest
{
    [Fact]
    public async Task DirectoryManagerCanCreateEmployee()
    {
        var directoryPage = new EmployeeDirectoryPage(Page);
        var createEmployeePage = new CreateEmployeePage(Page);
        var createWorkflow = new EmployeeCreationWorkflow(
            directoryPage,
            createEmployeePage);
        var searchWorkflow = new EmployeeSearchWorkflow(directoryPage);

        await RunReportedTestAsync(
            nameof(DirectoryManagerCanCreateEmployee),
            async report =>
            {
                await report.StepAsync(
                    "Open employee directory",
                    directoryPage.OpenAsync);
                await report.StepAsync(
                    "Reset predictable employee data",
                    directoryPage.ResetDataAsync);
                await report.StepAsync(
                    "Create employee as directory manager",
                    () => createWorkflow.CreateAsync(
                        TestRoles.DirectoryManager,
                        EmployeeCreationData.AutomationEngineer));
                await report.StepAsync(
                    "Verify created employee is searchable",
                    () => searchWorkflow.SearchAndVerifyAsync(
                        TestRoles.DirectoryManager,
                        EmployeeCreationData.AutomationEngineer));
            },
            async () =>
            {
                await directoryPage.OpenAsync();
                await directoryPage.ResetDataAsync();
            });
    }
}
