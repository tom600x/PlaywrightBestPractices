using Microsoft.Playwright;

namespace EmployeeDirectory.EngineeredTests.Components;

public sealed class SearchPanel(IPage page)
{
    private ILocator SearchInput => page.GetByTestId("search-input");
    private ILocator SearchButton => page.GetByTestId("search-button");

    public async Task SearchAsync(string searchTerm)
    {
        await SearchInput.FillAsync(searchTerm);
        await SearchButton.ClickAsync();
    }
}
