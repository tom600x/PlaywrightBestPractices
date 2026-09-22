using EmployeeDirectory.EngineeredTests.Components;
using Microsoft.Playwright;

namespace EmployeeDirectory.EngineeredTests.Pages;

public sealed class CreateEmployeePage
{
    private readonly IPage _page;

    public CreateEmployeePage(IPage page)
    {
        _page = page;
        Form = new EmployeeForm(page);
    }

    public EmployeeForm Form { get; }
    public ILocator PageTitle => _page.GetByTestId("page-title");
}
