using System.Globalization;
using EmployeeDirectory.EngineeredTests.Models;
using Microsoft.Playwright;

namespace EmployeeDirectory.EngineeredTests.Components;

public sealed class EmployeeForm(IPage page)
{
    public async Task FillAsync(EmployeeTestData employee)
    {
        await page.GetByTestId("input-firstname").FillAsync(employee.FirstName);
        await page.GetByTestId("input-lastname").FillAsync(employee.LastName);
        await page.GetByTestId("input-email").FillAsync(employee.Email);
        await page.GetByTestId("input-title").FillAsync(employee.Title);
        await page.GetByTestId("input-department").SelectOptionAsync(
            new SelectOptionValue { Label = employee.Department.ToString() });
        await page.GetByTestId("input-status").SelectOptionAsync(
            new SelectOptionValue { Label = employee.Status.ToString() });
        await page.GetByTestId("input-hiredate").FillAsync(
            employee.HireDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
    }

    public Task SaveAsync() => page.GetByTestId("save-button").ClickAsync();
}
