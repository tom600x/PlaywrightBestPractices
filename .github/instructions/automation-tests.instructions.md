---
applyTo: "EmployeeDirectory.AutomationTests/**/*"
---

# Basic Automation Tests

- Keep this project intentionally simple for teaching first-stage Playwright automation.
- Use xUnit and `Microsoft.Playwright.Xunit`; target `http://localhost:5080`.
- Prefer role/label locators, then `data-testid`; use CSS only for explicit comparison examples.
- Keep tests independent and restore sample data after stateful scenarios.
- Update the project README when adding or changing a test.
