using EmployeeDirectory.EngineeredTests.Data.Employees;
using EmployeeDirectory.EngineeredTests.Framework;
using EmployeeDirectory.EngineeredTests.Pages;
using EmployeeDirectory.EngineeredTests.Roles;
using EmployeeDirectory.EngineeredTests.Workflows;

namespace EmployeeDirectory.EngineeredTests.Tests.Search;

public sealed class EmployeeSearchTests : EngineeredPageTest
{
    [Fact]
    public async Task DirectoryManagerCanFindEmployeeByEmail()
    {
        var directoryPage = new EmployeeDirectoryPage(Page);
        var workflow = new EmployeeSearchWorkflow(directoryPage);

        await RunReportedTestAsync(
            nameof(DirectoryManagerCanFindEmployeeByEmail),
            async report =>
            {
                await report.StepAsync(
                    "Open employee directory",
                    directoryPage.OpenAsync);
                await report.StepAsync(
                    "Reset predictable employee data",
                    directoryPage.ResetDataAsync);
                await report.StepAsync(
                    "Search for seeded employee by email",
                    () => workflow.SearchAndVerifyAsync(
                        TestRoles.DirectoryManager,
                        SeededEmployees.Engineer));
            });
    }
}
