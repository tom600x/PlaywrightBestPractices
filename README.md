# Playwright Best Practices

This repository demonstrates how to build maintainable end-to-end UI test
automation with [Playwright](https://playwright.dev/) and .NET. It uses a
single ASP.NET Core MVC sample application as the system under test, then
shows two different ways of writing Playwright tests against it: a simple,
intentionally basic first draft, and a fully modular, engineered approach.

## Why this repository exists

Teams adopting Playwright often start with quick, working tests and later
struggle to keep them maintainable as the application and test suite grow.
This repository is a teaching tool that answers three questions:

1. What does a typical "first draft" Playwright test look like, and what are
   its limitations as a suite grows?
2. How do you evolve those tests into a modular, scalable automation
   architecture using pages, components, workflows, roles, and business
   rules?
3. What makes an application itself easier or harder to automate (stable
   `data-testid` locators, deterministic seed data, a reset operation,
   explicit validation messages)?

## Projects in this repository

This repository contains three projects that work together:

| Project | Purpose |
|---|---|
| [`EmployeeDirectory`](EmployeeDirectory) | The ASP.NET Core MVC application under test. It is an in-memory employee directory with search, employee creation, and employment-status workflows, built to be testable with stable `data-testid` attributes and a deterministic reset operation. |
| [`EmployeeDirectory.AutomationTests`](EmployeeDirectory.AutomationTests/README.md) | An intentionally simple, first-draft xUnit/Playwright test project. It shows what automation commonly looks like before tests are split into reusable pages, components, and workflows, and it compares fragile CSS locators with stable `data-testid` locators. |
| [`EmployeeDirectory.EngineeredTests`](EmployeeDirectory.EngineeredTests/README.md) | A modular, scalable xUnit/Playwright test project. It demonstrates page objects, reusable components, business workflows, roles, business rules, organized test data, and per-test HTML reports, screenshots, and traces. |

For a guided walkthrough that compares the two test projects side by side,
see the [Playwright Test Engineering Demo Guide](PLAYWRIGHT-DEMO-GUIDE.md).

## Getting started

1. Start the Employee Directory application:

   ```powershell
   dotnet run --project EmployeeDirectory/EmployeeDirectory.csproj
   ```

2. Run the basic automation project (see its
   [README](EmployeeDirectory.AutomationTests/README.md) for full details):

   ```powershell
   dotnet test EmployeeDirectory.AutomationTests
   ```

3. Run the engineered automation project (see its
   [README](EmployeeDirectory.EngineeredTests/README.md) for full details):

   ```powershell
   dotnet test EmployeeDirectory.EngineeredTests
   ```

## Where to go next

- Read the [Employee Directory](EmployeeDirectory) source to see how the
  application is designed for testability.
- Read the [`EmployeeDirectory.AutomationTests` README](EmployeeDirectory.AutomationTests/README.md)
  to see a simple starting point and locator selection guidance.
- Read the [`EmployeeDirectory.EngineeredTests` README](EmployeeDirectory.EngineeredTests/README.md)
  to see how tests scale using pages, components, workflows, roles, and rules.
- Read the [Playwright Test Engineering Demo Guide](PLAYWRIGHT-DEMO-GUIDE.md)
  for a full narrative comparing both approaches.
