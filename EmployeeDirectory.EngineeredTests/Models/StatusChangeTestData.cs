namespace EmployeeDirectory.EngineeredTests.Models;

public sealed record StatusChangeTestData(
    EmployeeStatus NewStatus,
    DateOnly EffectiveDate,
    string Reason,
    string Notes);
