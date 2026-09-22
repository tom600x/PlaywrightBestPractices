# Employee Directory Engineered Tests

This project demonstrates the move from test creation to test engineering:

- Tests are grouped by functional area.
- Reusable components model stable `data-testid` UI controls.
- Page objects model application pages.
- Workflows model business operations rather than individual screens.
- Roles and business rules make actor capabilities explicit.
- Predictable test data is grouped into seeded employees, creation scenarios,
  and employment-status scenarios.
- Strongly typed departments, statuses, and dates prevent invalid test data.
- Every test generates an HTML report, screenshots, and a Playwright trace.

The employment lifecycle area demonstrates status transitions, required reasons,
immutable history verification, and rejection of invalid no-op transitions.

## Reuse of test pages

Tests compose reusable pages and workflows instead of redefining locators and
navigation in every test.

| Reusable page | Responsibility | Reused by |
|---|---|---|
| `EmployeeDirectoryPage` | Opens the directory, resets predictable data, exposes search/results, and starts employee creation | Search, creation, and all employment-status tests |
| `CreateEmployeePage` | Exposes the reusable employee form on the Add Employee page | Employee creation tests |
| `EmployeeDetailsPage` | Exposes current status, status history, and status-workflow navigation | All employment-status tests |
| `ChangeStatusPage` | Exposes the reusable status-change form and current-status display | All employment-status tests |

The pages are composed from smaller reusable components:

| Component | Owned by page | Purpose |
|---|---|---|
| `SearchPanel` | `EmployeeDirectoryPage` | Enters and submits employee searches |
| `EmployeeTable` | `EmployeeDirectoryPage` | Verifies result counts and opens an employee by stable row content |
| `EmployeeForm` | `CreateEmployeePage` | Enters employee creation data and saves it |
| `StatusChangeForm` | `ChangeStatusPage` | Enters status, effective date, reason, and notes |
| `StatusHistoryTable` | `EmployeeDetailsPage` | Verifies immutable lifecycle history |

Workflows then compose multiple pages into business operations:

- `EmployeeSearchWorkflow` reuses `EmployeeDirectoryPage`.
- `EmployeeCreationWorkflow` reuses `EmployeeDirectoryPage` and
  `CreateEmployeePage`.
- `EmploymentStatusWorkflow` reuses `EmployeeDirectoryPage`,
  `EmployeeDetailsPage`, and `ChangeStatusPage`.

For example, the creation test creates each page once and passes those same page
objects into reusable workflows:

```csharp
var directoryPage = new EmployeeDirectoryPage(Page);
var createEmployeePage = new CreateEmployeePage(Page);

var createWorkflow = new EmployeeCreationWorkflow(
    directoryPage,
    createEmployeePage);
var searchWorkflow = new EmployeeSearchWorkflow(directoryPage);

await createWorkflow.CreateAsync(
    TestRoles.DirectoryManager,
    EmployeeCreationData.AutomationEngineer);

await searchWorkflow.SearchAndVerifyAsync(
    TestRoles.DirectoryManager,
    EmployeeCreationData.AutomationEngineer);
```

The status test follows the same composition pattern:

```csharp
var directoryPage = new EmployeeDirectoryPage(Page);
var detailsPage = new EmployeeDetailsPage(Page);
var changeStatusPage = new ChangeStatusPage(Page);

var workflow = new EmploymentStatusWorkflow(
    directoryPage,
    detailsPage,
    changeStatusPage);
```

As a result, stable `data-testid` locators are defined once in components or
pages. Tests remain focused on roles, business rules, and expected outcomes.

## Test data organization

Canonical fixtures are grouped by purpose rather than accumulated in one global
data class:

- `Data/Employees/SeededEmployees.cs` contains records restored by the
  application's reset operation.
- `Data/Employees/EmployeeCreationData.cs` contains employees created by tests.
- `Data/Status/EmploymentStatusData.cs` contains valid and invalid lifecycle
  scenarios.

The data records use `Department`, `EmployeeStatus`, and `DateOnly` values.
Components convert those values to the exact strings required by the UI. Add a
builder only when several tests need small variations of the same employee;
keep important business scenarios as explicitly named data.

## Test catalog

### Employee search

#### `DirectoryManagerCanFindEmployeeByEmail`

Verifies that a directory manager can:

- Reset the directory to predictable seed data.
- Search for a seeded employee by email address.
- See exactly one matching employee.
- Verify the employee's name, email, and title in the results.

### Employee creation

#### `DirectoryManagerCanCreateEmployee`

Verifies that a directory manager can:

- Reset the directory to predictable seed data.
- Open the Add Employee workflow.
- Enter valid employee details.
- Save the employee successfully.
- Search for and verify the newly created employee.
- Reset the directory during cleanup.

### Employment status

#### `DirectoryManagerCanPlaceActiveEmployeeOnLeave`

Verifies that a directory manager can:

- Find an active employee.
- Open the employment status workflow.
- Change the employee from `Active` to `OnLeave`.
- Supply an effective date, reason, and notes.
- Verify the new current status.
- Verify the immutable status-history entry.

#### `LeaveStatusRequiresReason`

Verifies that:

- An `OnLeave` transition without a reason is rejected.
- The required-reason validation message is displayed.
- The employee remains `Active`.

#### `NewStatusMustDifferFromCurrentStatus`

Verifies that:

- Selecting the employee's existing status is rejected.
- The no-change business-rule message is displayed.
- The employee's current status remains unchanged.

#### `TerminatedEmployeeCannotBeReactivated`

Verifies that:

- A terminated employee has no Change Status button.
- Direct navigation to the status form does not bypass server-side rules.
- An attempted transition from `Terminated` to `Active` is rejected.
- The employee remains `Terminated`.

## Run

Start the Employee Directory application on `http://localhost:5080`:

```powershell
dotnet run --project ..\EmployeeDirectory\EmployeeDirectory.csproj
```

Install Chromium once:

```powershell
dotnet build
.\bin\Debug\net10.0\playwright.ps1 install chromium
```

Run the tests:

```powershell
dotnet test
```

Artifacts are written to:

```text
reports\<yyyy-MM-dd>\<test-name>\
  report.html
  trace.zip
  01-*.png
  02-*.png
```
