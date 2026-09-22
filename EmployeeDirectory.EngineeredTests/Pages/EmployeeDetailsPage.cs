using EmployeeDirectory.EngineeredTests.Components;
using Microsoft.Playwright;

namespace EmployeeDirectory.EngineeredTests.Pages;

public sealed class EmployeeDetailsPage
{
    private readonly IPage _page;

    public EmployeeDetailsPage(IPage page)
    {
        _page = page;
        History = new StatusHistoryTable(page);
    }

    public ILocator CurrentStatus => _page.GetByTestId("current-status");
    public ILocator StatusMessage => _page.GetByTestId("status-message");
    public ILocator ChangeStatusButton => _page.GetByTestId("change-status-button");
    public StatusHistoryTable History { get; }

    public Task OpenChangeStatusAsync() =>
        ChangeStatusButton.ClickAsync();

    public Task OpenChangeStatusDirectlyAsync()
    {
        var statusUrl = _page.Url.Replace(
            "/Details/",
            "/ChangeStatus/",
            StringComparison.OrdinalIgnoreCase);
        return _page.GotoAsync(statusUrl);
    }
}
