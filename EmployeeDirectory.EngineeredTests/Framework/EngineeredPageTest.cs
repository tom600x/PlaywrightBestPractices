using EmployeeDirectory.EngineeredTests.Reporting;
using Microsoft.Playwright.Xunit;

namespace EmployeeDirectory.EngineeredTests.Framework;

public abstract class EngineeredPageTest : PageTest
{
    protected async Task RunReportedTestAsync(
        string testName,
        Func<TestReport, Task> test,
        Func<Task>? cleanup = null)
    {
        var report = new TestReport(Page, Context, testName);
        await report.StartAsync();

        Exception? testFailure = null;

        try
        {
            await test(report);
        }
        catch (Exception exception)
        {
            testFailure = exception;
            await report.RecordFailureAsync(exception);
        }

        try
        {
            if (cleanup is not null)
            {
                await cleanup();
            }
        }
        catch (Exception cleanupException)
        {
            testFailure ??= cleanupException;
            await report.RecordFailureAsync(cleanupException, "Test cleanup");
        }
        finally
        {
            await report.CompleteAsync(testFailure);
        }

        if (testFailure is not null)
        {
            System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(testFailure).Throw();
        }
    }
}
