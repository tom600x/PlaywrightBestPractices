using EmployeeDirectory.EngineeredTests.Components;
using EmployeeDirectory.EngineeredTests.Framework;
using Microsoft.Playwright;

namespace EmployeeDirectory.EngineeredTests.Pages;

public sealed class EmployeeDirectoryPage
{
    private readonly IPage _page;

    public EmployeeDirectoryPage(IPage page)
    {
        _page = page;
        Search = new SearchPanel(page);
        Employees = new EmployeeTable(page);
    }

    public SearchPanel Search { get; }
    public EmployeeTable Employees { get; }
    public ILocator PageTitle => _page.GetByTestId("page-title");
    public ILocator StatusMessage => _page.GetByTestId("status-message");

    public Task OpenAsync() => _page.GotoAsync(TestConfiguration.BaseUrl);

    public Task OpenCreateEmployeeAsync() =>
        _page.GetByTestId("add-employee-button").ClickAsync();

    public async Task ResetDataAsync()
    {
        void AcceptResetDialog(object? _, IDialog dialog) => _ = dialog.AcceptAsync();

        _page.Dialog += AcceptResetDialog;
        try
        {
            await _page.GetByTestId("reset-data-button").ClickAsync();
            await Assertions.Expect(Employees.ResultCount).ToHaveTextAsync("8 employees found");
        }
        finally
        {
            _page.Dialog -= AcceptResetDialog;
        }
    }
}
