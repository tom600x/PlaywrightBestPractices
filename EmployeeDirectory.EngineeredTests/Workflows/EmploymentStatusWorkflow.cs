using System.Globalization;
using EmployeeDirectory.EngineeredTests.Models;
using EmployeeDirectory.EngineeredTests.Pages;
using EmployeeDirectory.EngineeredTests.Roles;
using EmployeeDirectory.EngineeredTests.Rules;
using Microsoft.Playwright;

namespace EmployeeDirectory.EngineeredTests.Workflows;

public sealed class EmploymentStatusWorkflow(
    EmployeeDirectoryPage directoryPage,
    EmployeeDetailsPage detailsPage,
    ChangeStatusPage changeStatusPage)
{
    public async Task OpenEmployeeStatusAsync(
        DirectoryActor actor,
        EmployeeTestData employee)
    {
        EmployeeBusinessRules.EnsureCanSearch(actor);
        EmployeeBusinessRules.EnsureCanChangeStatus(actor);

        await directoryPage.Search.SearchAsync(employee.Email);
        await directoryPage.Employees.OpenDetailsByEmailAsync(employee.Email);
        await Assertions.Expect(detailsPage.CurrentStatus)
            .ToHaveTextAsync(employee.Status.ToString());
        await detailsPage.OpenChangeStatusAsync();
        await Assertions.Expect(changeStatusPage.PageTitle)
            .ToHaveTextAsync("Change Employment Status");
    }

    public async Task ChangeAndVerifyAsync(
        StatusChangeTestData statusChange,
        EmployeeStatus previousStatus)
    {
        var previousStatusText = previousStatus.ToString();
        var newStatusText = statusChange.NewStatus.ToString();

        await changeStatusPage.Form.FillAsync(statusChange);
        await changeStatusPage.Form.SaveAsync();

        await Assertions.Expect(detailsPage.CurrentStatus)
            .ToHaveTextAsync(newStatusText);
        await Assertions.Expect(detailsPage.StatusMessage)
            .ToContainTextAsync(
                $"changed from {previousStatusText} to {newStatusText}");
        await Assertions.Expect(detailsPage.History.Table)
            .ToContainTextAsync(previousStatusText);
        await Assertions.Expect(detailsPage.History.Table)
            .ToContainTextAsync(newStatusText);
        await Assertions.Expect(detailsPage.History.Table)
            .ToContainTextAsync(statusChange.Reason);
        await Assertions.Expect(detailsPage.History.Table)
            .ToContainTextAsync(
                statusChange.EffectiveDate.ToString(
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture));
    }

    public async Task OpenTerminatedEmployeeStatusDirectlyAsync(
        DirectoryActor actor,
        EmployeeTestData employee)
    {
        EmployeeBusinessRules.EnsureCanSearch(actor);
        EmployeeBusinessRules.EnsureCanChangeStatus(actor);

        await directoryPage.Search.SearchAsync(employee.Email);
        await directoryPage.Employees.OpenDetailsByEmailAsync(employee.Email);
        await Assertions.Expect(detailsPage.CurrentStatus)
            .ToHaveTextAsync(EmployeeStatus.Terminated.ToString());
        await Assertions.Expect(detailsPage.ChangeStatusButton)
            .ToHaveCountAsync(0);

        await detailsPage.OpenChangeStatusDirectlyAsync();
        await Assertions.Expect(changeStatusPage.PageTitle)
            .ToHaveTextAsync("Change Employment Status");
    }
}
