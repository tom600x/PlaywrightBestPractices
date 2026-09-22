using System.Globalization;
using EmployeeDirectory.EngineeredTests.Models;
using Microsoft.Playwright;

namespace EmployeeDirectory.EngineeredTests.Components;

public sealed class StatusChangeForm(IPage page)
{
    public ILocator NewStatus => page.GetByTestId("new-status");
    public ILocator EffectiveDate => page.GetByTestId("effective-date");
    public ILocator Reason => page.GetByTestId("status-reason");
    public ILocator Notes => page.GetByTestId("status-notes");
    public ILocator SaveButton => page.GetByTestId("save-status-button");
    public ILocator ReasonError => page.GetByTestId("error-status-reason");
    public ILocator NewStatusError => page.GetByTestId("error-new-status");

    public async Task FillAsync(StatusChangeTestData statusChange)
    {
        await NewStatus.SelectOptionAsync(
            new SelectOptionValue { Label = statusChange.NewStatus.ToString() });
        await EffectiveDate.FillAsync(
            statusChange.EffectiveDate.ToString(
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture));
        await Reason.FillAsync(statusChange.Reason);
        await Notes.FillAsync(statusChange.Notes);
    }

    public Task SaveAsync() => SaveButton.ClickAsync();
}
