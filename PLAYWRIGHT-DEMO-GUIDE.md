# Playwright Test Engineering Demo Guide

This presentation compares two Employee Directory automation projects and
explains how a team can progress from Playwright tests that work to a
maintainable automation architecture for a large application.

## Demo goals

By the end of the demonstration, the audience should understand:

- Why application testability matters.
- Where Playwright codegen helps and where its responsibility ends.
- How to select maintainable locators.
- Why page objects alone are not a complete test architecture.
- How components, pages, workflows, roles, rules, and test data work together.
- Why deterministic state, cleanup, reports, screenshots, and traces are
  essential for a large suite.

## Projects used in the demonstration

### Employee Directory

The ASP.NET Core MVC application used as the system under test:

```text
EmployeeDirectory
```

### Basic automation project

The intentionally simple starting point:

```text
EmployeeDirectory.AutomationTests
```

### Engineered automation project

The modular and scalable implementation:

```text
EmployeeDirectory.EngineeredTests
```

## 1. Begin with the application

Briefly demonstrate the Employee Directory:

- Search for an employee.
- Add an employee.
- Change employment status.
- Show status history.
- Reset sample data.

Explain that the application was deliberately designed for automation:

- Stable `data-testid` attributes
- Predictable in-memory seed data
- A reset operation
- Explicit validation messages
- Repeatable business workflows

> Test automation quality begins with application testability, not with the
> test framework.

## 2. Demonstrate the basic automation project

Open:

```text
EmployeeDirectory.AutomationTests
```

Use `EmployeeDirectoryTests.cs` to show a typical starting point.

### Strengths

- Easy to create
- Easy to understand
- Useful for prototypes and learning
- Provides immediate end-to-end coverage
- Runs in a visible browser
- Uses Playwright assertions and automatic waiting

### Scaling problems

The test method handles everything:

- Navigation
- Locators
- Test data
- Search behavior
- Employee creation
- Assertions
- Cleanup

As the application grows, this can lead to:

- Duplicated locators
- Duplicated navigation
- Large test methods
- Inconsistent cleanup
- Difficult maintenance
- Changes requiring edits across many tests
- Tests organized around screens instead of business behavior

> A long test is not necessarily a comprehensive test. It may simply contain
> too many responsibilities.

## 3. Demonstrate the locator comparison

Run:

```powershell
dotnet test --filter "FullyQualifiedName~CompareCssLocatorsWithStableTestIdLocators"
```

### First-draft locator approach

```csharp
Page.Locator("#searchForm button[type='submit']")
Page.Locator("table tbody tr")
Page.Locator("p.text-muted")
```

These selectors are coupled to implementation details:

- HTML structure
- Element types
- CSS presentation
- DOM hierarchy

### Stable test-contract approach

```csharp
Page.GetByTestId("search-input")
Page.GetByTestId("search-button")
Page.GetByTestId("result-count")
```

These selectors represent an explicit automation contract.

### Recommended locator priority

1. Accessible role and name
2. Labels or stable user-facing text
3. Stable `data-testid`
4. CSS or XPath only when necessary

Examples:

```csharp
Page.GetByRole(AriaRole.Button, new() { Name = "Search" });
Page.GetByLabel("Email");
Page.GetByTestId("employee-table");
```

> Use locators that describe intent. Do not encode the page's current
> implementation into every test.

## 4. Explain the proper role of codegen

Playwright codegen is valuable for:

- Discovering initial locators
- Capturing an unfamiliar workflow
- Learning the Playwright API
- Producing a first draft
- Quickly confirming that a flow can be automated

Generated code is not generally the final framework artifact.

The normal refinement process should be:

```text
Record flow
    |
    v
Understand behavior
    |
    v
Replace fragile locators
    |
    v
Extract reusable components
    |
    v
Model workflows and business rules
    |
    v
Add predictable data and cleanup
    |
    v
Add reporting and diagnostics
```

> Codegen accelerates test creation. Engineering makes the resulting tests
> maintainable.

## 5. Move to the engineered project

Open:

```text
EmployeeDirectory.EngineeredTests
```

Show the project responsibilities:

```text
Tests/
    Business scenarios and expected outcomes

Workflows/
    Multi-page business operations

Pages/
    Page-level capabilities

Components/
    Reusable UI controls

Roles/
    Actor permissions and capabilities

Rules/
    Business requirements

Models/
    Strongly typed employees, departments, statuses, and status changes

Data/
    Predictable seeded, creation, and employment-status scenarios

Framework/
    Shared test execution, cleanup, configuration, and report lifecycle

Reporting/
    HTML, screenshots, and traces
```

Each layer has one clear purpose.

### Show the strongly typed test contract

Open:

```text
EmployeeDirectory.EngineeredTests\Models\EmployeeTestData.cs
EmployeeDirectory.EngineeredTests\Models\StatusChangeTestData.cs
EmployeeDirectory.EngineeredTests\Data
```

The engineered suite no longer passes loosely related strings through tests.
It models employees and status changes with:

- `Department` and `EmployeeStatus` enums
- `DateOnly` values for hire and effective dates
- Named seeded, creation, and employment-status scenarios
- A derived `FullName` instead of repeated string construction

Components own the conversion from domain values to UI values. For example,
`EmployeeForm` and `StatusChangeForm` convert enums to option labels and dates
to the invariant `yyyy-MM-dd` format expected by HTML date inputs.

Benefits include:

- Invalid department and status values are prevented at compile time.
- Dates are not confused with arbitrary strings or timestamps.
- Tests share named business scenarios without sharing browser mechanics.
- UI formatting remains centralized in components.

> Strong test data is part of the automation design. It makes invalid states
> harder to express and keeps UI serialization out of business scenarios.

## 6. Demonstrate page reuse

Use the employment-status tests in:

```text
EmployeeDirectory.EngineeredTests\Tests\Status\EmploymentStatusTests.cs
```

Multiple scenarios reuse:

- `EmployeeDirectoryPage`
- `EmployeeDetailsPage`
- `ChangeStatusPage`
- `EmploymentStatusWorkflow`
- `StatusChangeForm`
- `StatusHistoryTable`

The tests describe business intent:

- Place an active employee on leave.
- Require a leave reason.
- Reject an unchanged status.
- Prevent reactivating a terminated employee.

Instead of repeatedly writing browser mechanics:

```csharp
await Page.Locator(...).ClickAsync();
await Page.Locator(...).FillAsync(...);
```

The tests invoke business operations:

```csharp
await workflow.OpenEmployeeStatusAsync(...);
await workflow.ChangeAndVerifyAsync(...);
```

> Tests should say what the business is doing. Pages and components should know
> how the browser does it.

## 7. Explain why workflows matter

A page-object-only design can still become difficult to maintain if tests
manually coordinate many pages.

The employee status journey crosses:

1. Directory search
2. Search results
3. Employee details
4. Status form
5. Status history

`EmploymentStatusWorkflow` encapsulates that journey.

Benefits include:

- One implementation of the business flow
- Reuse across positive and negative scenarios
- Less duplicated navigation
- Consistent assertions
- Easier adaptation when the workflow changes

## 8. Demonstrate predictable state

Show the Reset Sample Data operation.

Large automation suites need controlled state because:

- Tests must not depend on execution order.
- Test reruns must be safe.
- Failed tests must not poison later tests.
- Parallel execution requires isolated or deterministic data.
- Cleanup should run even when an assertion fails.

This sample uses:

- Seed data with known employees
- Reset to restore the initial state
- Named, strongly typed fixtures with fixed values and dates
- Guaranteed cleanup callbacks for stateful scenarios

The basic project creates a unique employee to reduce collision risk. The
engineered project instead uses a fixed creation scenario and resets the
in-memory repository before and after the stateful test. This makes the
scenario, report, and expected results identical on every run.

`EngineeredPageTest.RunReportedTestAsync` keeps test execution and cleanup
separate. Cleanup still runs after an assertion failure, and a cleanup failure
is recorded rather than silently ignored.

Production alternatives may include:

- API-based data setup
- Database fixture creation
- Disposable test tenants
- Seed scripts
- Containerized databases
- Test-specific service endpoints

> UI automation should verify behavior through the UI, but it does not need to
> perform all setup through the UI.

## 9. Demonstrate reports and diagnostics

Run:

```powershell
cd EmployeeDirectory.EngineeredTests
dotnet test
```

Then open:

```text
EmployeeDirectory.EngineeredTests\reports
```

Each test produces:

- `report.html`
- `trace.zip`
- Step screenshots

### Purpose of each artifact

- **HTML report:** A readable business-step summary
- **Screenshots:** Visual evidence at meaningful checkpoints
- **Trace:** Technical investigation with DOM snapshots, network activity, and
  Playwright actions

Show how every engineered test inherits from `EngineeredPageTest` and calls
`RunReportedTestAsync`. The shared lifecycle:

1. Starts tracing before the scenario.
2. Records named business steps and captures a screenshot after each step.
3. Captures failure details and a failure screenshot.
4. Runs optional cleanup even when the scenario fails.
5. Stops tracing and writes the HTML report before rethrowing the original
   failure to xUnit.

This keeps diagnostics consistent without duplicating reporting code in every
test.

> A failed test must provide enough evidence to diagnose the problem without
> immediately rerunning it locally.

## 10. Explain functional areas versus pages

Avoid organizing a large suite only around screens:

```text
LoginPageTests
SearchPageTests
DetailsPageTests
FormPageTests
```

Pages are implementation surfaces. Functional areas represent business
capabilities:

```text
Authentication
EmployeeSearch
EmployeeCreation
EmploymentStatus
DepartmentAssignment
Permissions
ImportExport
```

A functional test may cross several pages.

> Organize tests around capabilities owned by the business, not around URLs
> owned by the UI.

## Basic versus engineered automation

| Basic automation project | Engineered automation project |
|---|---|
| One large test class | Tests grouped by functional area |
| Locators inside test methods | Locators centralized in components and pages |
| Test controls navigation | Workflows control business journeys |
| Test creates inline data | Shared predictable scenario data |
| Strings represent departments, statuses, and dates | Typed enums, records, and `DateOnly` values |
| Limited separation of concerns | Clear responsibility boundaries |
| Good for learning and prototypes | Designed for long-term growth |
| Failures primarily shown by the runner | HTML reports, screenshots, and traces |
| Cleanup is implemented inside the test | Shared lifecycle runs and reports cleanup |
| Changes may affect many tests | UI changes are localized |
| Tests describe browser operations | Tests describe business outcomes |

## Core messages for the audience

1. Playwright is the browser automation engine, not the complete test
   architecture.
2. Codegen creates drafts; engineers create maintainable tests.
3. Good locators express user or automation intent.
4. Page objects remove UI duplication, but workflows model the business.
5. Tests should be independent and deterministic.
6. Application testability is a shared responsibility between developers and
   testers.
7. Reporting and traces are essential for operating a large suite.
8. A framework should reduce repeated work without hiding Playwright
   completely.
9. Do not abstract everything immediately. Extract patterns that are genuinely
   reused.
10. Strongly typed test data prevents invalid scenarios and centralizes UI
    formatting.
11. The goal is not the largest number of tests. It is reliable evidence about
    business risk.

## Recommended live-demo sequence

1. Reset and browse the Employee Directory.
2. Run `SearchAndAddEmployee`.
3. Show how much responsibility exists in one test.
4. Run the locator comparison test.
5. Explain codegen and locator refinement.
6. Open the engineered project structure.
7. Run the six engineered tests.
8. Show the typed models and named scenario data.
9. Show page and workflow reuse.
10. Show `RunReportedTestAsync` handling steps, failures, and cleanup.
11. Open one HTML report.
12. Open its screenshots and trace.
13. Finish with the basic-versus-engineered comparison table.

## Closing statement

> The first project proves that Playwright can automate the application. The
> engineered project shows how a team can continue automating it for years.
