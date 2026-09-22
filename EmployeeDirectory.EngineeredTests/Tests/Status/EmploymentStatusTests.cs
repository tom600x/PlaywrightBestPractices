using EmployeeDirectory.EngineeredTests.Data.Employees;
using EmployeeDirectory.EngineeredTests.Data.Status;
using EmployeeDirectory.EngineeredTests.Framework;
using EmployeeDirectory.EngineeredTests.Pages;
using EmployeeDirectory.EngineeredTests.Roles;
using EmployeeDirectory.EngineeredTests.Workflows;
using Microsoft.Playwright;

namespace EmployeeDirectory.EngineeredTests.Tests.Status;

public sealed class EmploymentStatusTests : EngineeredPageTest
{
    [Fact]
    public async Task DirectoryManagerCanPlaceActiveEmployeeOnLeave()
    {
        var scenario = CreateScenario();

        await RunReportedTestAsync(
            nameof(DirectoryManagerCanPlaceActiveEmployeeOnLeave),
            async report =>
            {
                await report.StepAsync(
                    "Open employee directory",
                    scenario.DirectoryPage.OpenAsync);
                await report.StepAsync(
                    "Reset employee lifecycle data",
                    scenario.DirectoryPage.ResetDataAsync);
                await report.StepAsync(
                    "Open active employee status workflow",
                    () => scenario.Workflow.OpenEmployeeStatusAsync(
                        TestRoles.DirectoryManager,
                        SeededEmployees.Engineer));
                await report.StepAsync(
                    "Place employee on parental leave",
                    () => scenario.Workflow.ChangeAndVerifyAsync(
                        EmploymentStatusData.ParentalLeave,
                        SeededEmployees.Engineer.Status));
            },
            async () =>
            {
                await scenario.DirectoryPage.OpenAsync();
                await scenario.DirectoryPage.ResetDataAsync();
            });
    }

    [Fact]
    public async Task LeaveStatusRequiresReason()
    {
        var scenario = CreateScenario();

        await RunReportedTestAsync(
            nameof(LeaveStatusRequiresReason),
            async report =>
            {
                await report.StepAsync(
                    "Open employee directory",
                    scenario.DirectoryPage.OpenAsync);
                await report.StepAsync(
                    "Reset employee lifecycle data",
                    scenario.DirectoryPage.ResetDataAsync);
                await report.StepAsync(
                    "Open active employee status workflow",
                    () => scenario.Workflow.OpenEmployeeStatusAsync(
                        TestRoles.DirectoryManager,
                        SeededEmployees.Engineer));
                await report.StepAsync(
                    "Submit leave status without reason",
                    async () =>
                    {
                        await scenario.ChangeStatusPage.Form.FillAsync(
                            EmploymentStatusData.ParentalLeaveWithoutReason);
                        await scenario.ChangeStatusPage.Form.SaveAsync();
                        await Assertions.Expect(
                                scenario.ChangeStatusPage.Form.ReasonError)
                            .ToHaveTextAsync("A reason is required for a status change.");
                        await Assertions.Expect(scenario.ChangeStatusPage.CurrentStatus)
                            .ToHaveTextAsync(SeededEmployees.Engineer.Status.ToString());
                    });
            });
    }

    [Fact]
    public async Task NewStatusMustDifferFromCurrentStatus()
    {
        var scenario = CreateScenario();

        await RunReportedTestAsync(
            nameof(NewStatusMustDifferFromCurrentStatus),
            async report =>
            {
                await report.StepAsync(
                    "Open employee directory",
                    scenario.DirectoryPage.OpenAsync);
                await report.StepAsync(
                    "Reset employee lifecycle data",
                    scenario.DirectoryPage.ResetDataAsync);
                await report.StepAsync(
                    "Open active employee status workflow",
                    () => scenario.Workflow.OpenEmployeeStatusAsync(
                        TestRoles.DirectoryManager,
                        SeededEmployees.Engineer));
                await report.StepAsync(
                    "Reject unchanged employment status",
                    async () =>
                    {
                        await scenario.ChangeStatusPage.Form.FillAsync(
                            EmploymentStatusData.NoStatusChange);
                        await scenario.ChangeStatusPage.Form.SaveAsync();
                        await Assertions.Expect(
                                scenario.ChangeStatusPage.Form.NewStatusError)
                            .ToHaveTextAsync(
                                "New status must be different from the current status.");
                        await Assertions.Expect(scenario.ChangeStatusPage.CurrentStatus)
                            .ToHaveTextAsync(SeededEmployees.Engineer.Status.ToString());
                    });
            });
    }

    [Fact]
    public async Task TerminatedEmployeeCannotBeReactivated()
    {
        var scenario = CreateScenario();

        await RunReportedTestAsync(
            nameof(TerminatedEmployeeCannotBeReactivated),
            async report =>
            {
                await report.StepAsync(
                    "Open employee directory",
                    scenario.DirectoryPage.OpenAsync);
                await report.StepAsync(
                    "Reset employee lifecycle data",
                    scenario.DirectoryPage.ResetDataAsync);
                await report.StepAsync(
                    "Verify terminated employee has no status action",
                    () => scenario.Workflow.OpenTerminatedEmployeeStatusDirectlyAsync(
                        TestRoles.DirectoryManager,
                        SeededEmployees.TerminatedSupportLead));
                await report.StepAsync(
                    "Reject direct terminated employee reactivation",
                    async () =>
                    {
                        await scenario.ChangeStatusPage.Form.FillAsync(
                            EmploymentStatusData.Reactivation);
                        await scenario.ChangeStatusPage.Form.SaveAsync();
                        await Assertions.Expect(
                                scenario.ChangeStatusPage.Form.NewStatusError)
                            .ToHaveTextAsync(
                                "A terminated employee cannot transition to another status.");
                        await Assertions.Expect(scenario.ChangeStatusPage.CurrentStatus)
                            .ToHaveTextAsync(
                                SeededEmployees.TerminatedSupportLead.Status.ToString());
                    });
            });
    }

    private EmploymentStatusScenario CreateScenario()
    {
        var directoryPage = new EmployeeDirectoryPage(Page);
        var changeStatusPage = new ChangeStatusPage(Page);
        var workflow = new EmploymentStatusWorkflow(
            directoryPage,
            new EmployeeDetailsPage(Page),
            changeStatusPage);

        return new EmploymentStatusScenario(
            directoryPage,
            changeStatusPage,
            workflow);
    }

    private sealed record EmploymentStatusScenario(
        EmployeeDirectoryPage DirectoryPage,
        ChangeStatusPage ChangeStatusPage,
        EmploymentStatusWorkflow Workflow);
}
