# Employee Directory Automation Tests

This is the initial, intentionally simple xUnit browser automation project. It
shows what a first automation project commonly looks like before tests are
separated into reusable pages, components, workflows, and functional areas.

The project contains two interactive Playwright tests:

| Test | Purpose |
|---|---|
| `SearchAndAddEmployee` | Demonstrates one complete end-to-end test using stable `data-testid` locators |
| `CompareCssLocatorsWithStableTestIdLocators` | Compares fragile CSS/structural locators with stable automation-contract locators |

## Run

Start the Employee Directory application:

```powershell
dotnet run --project ..\EmployeeDirectory\EmployeeDirectory.csproj
```

Build the test project and install Chromium the first time:

```powershell
dotnet build
.\bin\Debug\net10.0\playwright.ps1 install chromium
```

Run all automation tests:

```powershell
dotnet test
```

The tests open Chromium interactively so you can watch each browser action. They
target `http://localhost:5080/`.

Run one test by name:

```powershell
dotnet test --filter "FullyQualifiedName~SearchAndAddEmployee"
```

## Tests

### `SearchAndAddEmployee`

**Purpose:** Demonstrate a complete browser journey in one default xUnit test.

**Precondition:** The Employee Directory application is running at
`http://localhost:5080/`.

**Test flow:**

1. Opens the Employee Directory.
2. Verifies that the Employee Directory page is displayed.
3. Accepts the reset confirmation dialog.
4. Resets the in-memory data to the original eight employees.
5. Searches for `Ethan`.
6. Verifies that exactly one employee is returned.
7. Verifies Ethan Brown's name and email address.
8. Opens the Add Employee page.
9. Creates a uniquely named automation employee.
10. Verifies the success message.
11. Searches for the new employee by email address.
12. Verifies the employee's name, email, and title.
13. Resets the sample data during cleanup.
14. Verifies that the original eight employees remain.

**Why the employee is unique:** The test adds the current Unix timestamp to the
last name and email address. This prevents a repeated run from colliding with
data created by an earlier interrupted test.

**Locators used:** This test uses stable `data-testid` selectors such as:

```text
page-title
reset-data-button
search-input
search-button
employee-table
add-employee-button
input-firstname
input-lastname
input-email
save-button
```

**Teaching point:** This test is easy to understand, but it contains search,
creation, verification, and cleanup in one method. The separate
`EmployeeDirectory.EngineeredTests` project demonstrates how to move these
responsibilities into reusable functional areas.

### `CompareCssLocatorsWithStableTestIdLocators`

**Purpose:** Show customers why a locator that merely works today is not
necessarily a maintainable Playwright locator.

**Test flow:**

1. Opens the Employee Directory.
2. Searches for Ethan using CSS and structural selectors.
3. Verifies the search result using table markup and a presentation class.
4. Returns to the Employee Directory.
5. Repeats the same search using stable `data-testid` selectors.
6. Verifies that both approaches find the same employee.

The application behavior is identical in both halves. Only the locator strategy
changes.

#### First-draft CSS and structural locators

```csharp
await Page.Locator("#searchTerm").FillAsync("Ethan");
await Page.Locator("#searchForm button[type='submit']").ClickAsync();

var resultRow = Page
    .Locator("table tbody tr")
    .Filter(new LocatorFilterOptions
    {
        HasText = "ethan.brown@example.com"
    });

await Expect(Page.Locator("p.text-muted"))
    .ToHaveTextAsync("1 employee found");
```

These locators can work, and codegen may produce similar first drafts, but they
are coupled to implementation details:

- `#searchTerm` depends on an HTML `id`.
- `#searchForm button[type='submit']` depends on form hierarchy and element type.
- `table tbody tr` depends on the current table markup.
- `p.text-muted` uses a presentation class rather than business intent.

A harmless UI refactor can break the test even when the employee-search
behavior still works.

#### Stable test-contract locators

```csharp
await Page.GetByTestId("search-input").FillAsync("Ethan");
await Page.GetByTestId("search-button").ClickAsync();

await Expect(Page.GetByTestId("result-count"))
    .ToHaveTextAsync("1 employee found");
await Expect(Page.GetByTestId("employee-table"))
    .ToContainTextAsync("Ethan Brown");
```

The `data-testid` values form an explicit contract between the application and
automation:

- Tests describe intent instead of HTML structure.
- CSS and layout can change without rewriting tests.
- Locator failures identify the missing application contract clearly.
- Shared page components can define each locator once.

Use codegen to discover a flow and generate a first draft. Then replace fragile
CSS or structural selectors with accessible role/label locators where they
represent user behavior, or stable `data-testid` locators where the application
needs an explicit automation contract.

## Locator selection guidance

Prefer locators in this order:

1. **Accessible role and name** when the locator represents how a user finds a
   control:

   ```csharp
   Page.GetByRole(AriaRole.Button, new() { Name = "Search" })
   ```

2. **Label, placeholder, or visible text** when these values are stable product
   language:

   ```csharp
   Page.GetByLabel("Email")
   ```

3. **`data-testid`** when the application needs a stable automation contract
   that should not change with layout or styling:

   ```csharp
   Page.GetByTestId("search-input")
   ```

4. **CSS or XPath** only when no stronger user-facing or explicit test contract
   is available.

Avoid selectors that depend on:

- Generated CSS classes
- Element position such as `nth-child`
- Deep DOM ancestry
- Visual styling classes
- Exact markup that has no business meaning

## Project scope

This project intentionally keeps tests in
`EmployeeDirectoryTests.cs` to demonstrate a basic starting point. For the
modular approach, including reusable pages, components, workflows, roles,
business rules, and per-test reports, see the sibling
`EmployeeDirectory.EngineeredTests` project.
