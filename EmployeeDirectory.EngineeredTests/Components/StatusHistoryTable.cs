using Microsoft.Playwright;

namespace EmployeeDirectory.EngineeredTests.Components;

public sealed class StatusHistoryTable(IPage page)
{
    public ILocator Table => page.GetByTestId("status-history-table");
    public ILocator EmptyMessage => page.GetByTestId("status-history-empty");
}
