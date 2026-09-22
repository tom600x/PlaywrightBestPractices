---
applyTo: "EmployeeDirectory.EngineeredTests/**/*"
---

# Engineered Automation Tests

- Organize tests by functional area and keep test methods focused on business outcomes.
- Put locators in components/pages and multi-page behavior in workflows; reuse existing abstractions.
- Model actor permissions in roles, validation in rules, and deterministic scenarios in data classes.
- Use stable role/label or `data-testid` locators; avoid structural CSS and XPath.  Identify missing `data-testid` attributes.
- Run stateful scenarios with reset/cleanup.
- Generate each test's HTML report, screenshots, and trace under `reports/<date>/<test-name>`.
- Update the test catalog and reuse documentation when adding tests.
