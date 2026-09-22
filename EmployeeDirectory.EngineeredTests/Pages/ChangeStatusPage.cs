using EmployeeDirectory.EngineeredTests.Components;
using Microsoft.Playwright;

namespace EmployeeDirectory.EngineeredTests.Pages;

public sealed class ChangeStatusPage
{
    private readonly IPage _page;

    public ChangeStatusPage(IPage page)
    {
        _page = page;
        Form = new StatusChangeForm(page);
    }

    public ILocator PageTitle => _page.GetByTestId("page-title");
    public ILocator CurrentStatus => _page.GetByTestId("current-status");
    public StatusChangeForm Form { get; }
}
