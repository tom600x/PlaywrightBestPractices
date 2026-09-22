using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;

namespace EmployeeDirectory.AutomationTests;

public class EmployeeDirectoryTests : PageTest
{
    private const string BaseUrl = "http://localhost:5080";

    [Fact]
    public async Task SearchAndAddEmployee()
    {
        await Page.GotoAsync(BaseUrl);
        await Expect(Page.GetByTestId("page-title")).ToHaveTextAsync("Employee Directory");

        Page.Dialog += async (_, dialog) => await dialog.AcceptAsync();
        await Page.GetByTestId("reset-data-button").ClickAsync();
        await Expect(Page.GetByTestId("status-message"))
            .ToContainTextAsync("Data was reset to the seeded sample set.");

        await Page.GetByTestId("search-input").FillAsync("Ethan");
        await Page.GetByTestId("search-button").ClickAsync();

        await Expect(Page.GetByTestId("result-count")).ToHaveTextAsync("1 employee found");
        await Expect(Page.GetByTestId("employee-table")).ToContainTextAsync("Ethan Brown");
        await Expect(Page.GetByTestId("employee-table")).ToContainTextAsync("ethan.brown@example.com");

        await Page.GetByTestId("add-employee-button").ClickAsync();
        await Expect(Page.GetByTestId("page-title")).ToHaveTextAsync("Add Employee");

        var uniqueValue = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var firstName = "Taylor";
        var lastName = $"Automation{uniqueValue}";
        var email = $"taylor.automation{uniqueValue}@example.com";

        await Page.GetByTestId("input-firstname").FillAsync(firstName);
        await Page.GetByTestId("input-lastname").FillAsync(lastName);
        await Page.GetByTestId("input-email").FillAsync(email);
        await Page.GetByTestId("input-title").FillAsync("Automation Test Engineer");
        await Page.GetByTestId("input-department").SelectOptionAsync(
            new SelectOptionValue { Label = "Engineering" });
        await Page.GetByTestId("input-status").SelectOptionAsync(
            new SelectOptionValue { Label = "Active" });
        await Page.GetByTestId("input-hiredate").FillAsync("2026-09-21");
        await Page.GetByTestId("save-button").ClickAsync();

        await Expect(Page.GetByTestId("status-message"))
            .ToContainTextAsync($"Employee \"{firstName} {lastName}\" was created successfully.");

        await Page.GetByTestId("search-input").FillAsync(email);
        await Page.GetByTestId("search-button").ClickAsync();

        await Expect(Page.GetByTestId("result-count")).ToHaveTextAsync("1 employee found");
        await Expect(Page.GetByTestId("employee-table")).ToContainTextAsync($"{firstName} {lastName}");
        await Expect(Page.GetByTestId("employee-table")).ToContainTextAsync(email);
        await Expect(Page.GetByTestId("employee-table")).ToContainTextAsync("Automation Test Engineer");

        await Page.GotoAsync(BaseUrl);
        await Page.GetByTestId("reset-data-button").ClickAsync();
        await Expect(Page.GetByTestId("result-count")).ToHaveTextAsync("8 employees found");
    }

    [Fact]
    public async Task CompareCssLocatorsWithStableTestIdLocators()
    {
        await Page.GotoAsync(BaseUrl);

        // Typical first-draft/codegen style: coupled to element IDs, form structure, and table markup.
        await Page.Locator("#searchTerm").FillAsync("Ethan");
        await Page.Locator("#searchForm button[type='submit']").ClickAsync();

        var cssResultRow = Page
            .Locator("table tbody tr")
            .Filter(new LocatorFilterOptions { HasText = "ethan.brown@example.com" });

        await Expect(Page.Locator("p.text-muted")).ToHaveTextAsync("1 employee found");
        await Expect(cssResultRow).ToContainTextAsync("Ethan Brown");
        await Expect(cssResultRow).ToContainTextAsync("QA Engineer");

        await Page.GotoAsync(BaseUrl);

        // Engineered style: locators express intent and are insulated from layout/CSS refactoring.
        await Page.GetByTestId("search-input").FillAsync("Ethan");
        await Page.GetByTestId("search-button").ClickAsync();

        await Expect(Page.GetByTestId("result-count")).ToHaveTextAsync("1 employee found");
        await Expect(Page.GetByTestId("employee-table")).ToContainTextAsync("Ethan Brown");
        await Expect(Page.GetByTestId("employee-table"))
            .ToContainTextAsync("ethan.brown@example.com");
        await Expect(Page.GetByTestId("employee-table")).ToContainTextAsync("QA Engineer");
    }
}
