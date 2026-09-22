using System.Net;
using System.Text;
using Microsoft.Playwright;

namespace EmployeeDirectory.EngineeredTests.Reporting;

public sealed class TestReport
{
    private readonly IPage _page;
    private readonly IBrowserContext _context;
    private readonly string _testName;
    private readonly string _reportDirectory;
    private readonly List<ReportStep> _steps = [];
    private readonly DateTimeOffset _startedAt = DateTimeOffset.Now;
    private int _stepNumber;

    public TestReport(IPage page, IBrowserContext context, string testName)
    {
        _page = page;
        _context = context;
        _testName = testName;
        _reportDirectory = Path.Combine(
            Framework.TestConfiguration.ReportsDirectory,
            DateTime.Now.ToString("yyyy-MM-dd"),
            SanitizePathSegment(testName));
    }

    public async Task StartAsync()
    {
        Directory.CreateDirectory(_reportDirectory);
        await _context.Tracing.StartAsync(new TracingStartOptions
        {
            Screenshots = true,
            Snapshots = true,
            Sources = true
        });
    }

    public async Task StepAsync(string name, Func<Task> action)
    {
        var startedAt = DateTimeOffset.Now;
        _stepNumber++;

        try
        {
            await action();
            var screenshot = await CaptureScreenshotAsync($"{_stepNumber:00}-{name}");
            _steps.Add(new ReportStep(name, "Passed", startedAt, DateTimeOffset.Now, screenshot, null));
        }
        catch (Exception exception)
        {
            var screenshot = await CaptureScreenshotAsync($"{_stepNumber:00}-{name}-failed");
            _steps.Add(new ReportStep(
                name,
                "Failed",
                startedAt,
                DateTimeOffset.Now,
                screenshot,
                exception.Message));
            throw;
        }
    }

    public async Task RecordFailureAsync(
        Exception exception,
        string name = "Unhandled test failure")
    {
        _stepNumber++;
        var screenshot = await CaptureScreenshotAsync($"{_stepNumber:00}-{name}-failed");
        _steps.Add(new ReportStep(
            name,
            "Failed",
            DateTimeOffset.Now,
            DateTimeOffset.Now,
            screenshot,
            exception.ToString()));
    }

    public async Task CompleteAsync(Exception? failure)
    {
        var tracePath = Path.Combine(_reportDirectory, "trace.zip");
        await _context.Tracing.StopAsync(new TracingStopOptions { Path = tracePath });

        var completedAt = DateTimeOffset.Now;
        var html = BuildHtml(
            failure is null ? "Passed" : "Failed",
            completedAt,
            Path.GetFileName(tracePath));
        await File.WriteAllTextAsync(
            Path.Combine(_reportDirectory, "report.html"),
            html);
    }

    private async Task<string> CaptureScreenshotAsync(string name)
    {
        var fileName = $"{SanitizePathSegment(name)}.png";
        await _page.ScreenshotAsync(new PageScreenshotOptions
        {
            Path = Path.Combine(_reportDirectory, fileName),
            FullPage = true
        });
        return fileName;
    }

    private string BuildHtml(
        string outcome,
        DateTimeOffset completedAt,
        string traceFileName)
    {
        var encodedTestName = WebUtility.HtmlEncode(_testName);
        var outcomeClass = outcome == "Passed" ? "passed" : "failed";
        var rows = new StringBuilder();

        foreach (var step in _steps)
        {
            var duration = step.CompletedAt - step.StartedAt;
            rows.AppendLine($"""
                <tr>
                  <td>{WebUtility.HtmlEncode(step.Name)}</td>
                  <td class="{step.Outcome.ToLowerInvariant()}">{step.Outcome}</td>
                  <td>{duration.TotalSeconds:F2}s</td>
                  <td><a href="{WebUtility.HtmlEncode(step.Screenshot)}">Screenshot</a></td>
                  <td><pre>{WebUtility.HtmlEncode(step.Error ?? string.Empty)}</pre></td>
                </tr>
                """);
        }

        return $$"""
            <!doctype html>
            <html lang="en">
            <head>
              <meta charset="utf-8">
              <title>{{encodedTestName}} - Test Report</title>
              <style>
                body { font-family: Segoe UI, Arial, sans-serif; margin: 2rem; color: #252525; }
                a { text-decoration: none; color: #464feb; }
                table { border-collapse: collapse; width: 100%; }
                tr th, tr td { border: 1px solid #e6e6e6; padding: .75rem; vertical-align: top; }
                tr th { background-color: #f5f5f5; text-align: left; }
                .passed { color: #107c10; font-weight: 700; }
                .failed { color: #d13438; font-weight: 700; }
                pre { margin: 0; max-width: 42rem; white-space: pre-wrap; }
              </style>
            </head>
            <body>
              <h1>{{encodedTestName}}</h1>
              <p>Outcome: <span class="{{outcomeClass}}">{{outcome}}</span></p>
              <p>Started: {{_startedAt:O}}<br>Completed: {{completedAt:O}}</p>
              <p><a href="{{traceFileName}}">Open Playwright trace</a></p>
              <table>
                <thead>
                  <tr><th>Step</th><th>Outcome</th><th>Duration</th><th>Evidence</th><th>Error</th></tr>
                </thead>
                <tbody>
                  {{rows}}
                </tbody>
              </table>
            </body>
            </html>
            """;
    }

    private static string SanitizePathSegment(string value)
    {
        var invalidCharacters = Path.GetInvalidFileNameChars();
        return string.Concat(value.Select(character =>
            invalidCharacters.Contains(character) ? '-' : character));
    }

    private sealed record ReportStep(
        string Name,
        string Outcome,
        DateTimeOffset StartedAt,
        DateTimeOffset CompletedAt,
        string Screenshot,
        string? Error);
}
