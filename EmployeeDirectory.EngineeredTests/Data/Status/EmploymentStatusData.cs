using EmployeeDirectory.EngineeredTests.Models;

namespace EmployeeDirectory.EngineeredTests.Data.Status;

public static class EmploymentStatusData
{
    private static readonly DateOnly EffectiveDate = new(2026, 9, 21);

    public static StatusChangeTestData ParentalLeave => new(
        EmployeeStatus.OnLeave,
        EffectiveDate,
        "Parental leave",
        "Expected return in twelve weeks.");

    public static StatusChangeTestData ParentalLeaveWithoutReason => new(
        EmployeeStatus.OnLeave,
        EffectiveDate,
        string.Empty,
        string.Empty);

    public static StatusChangeTestData NoStatusChange => new(
        EmployeeStatus.Active,
        EffectiveDate,
        "No actual change",
        string.Empty);

    public static StatusChangeTestData Reactivation => new(
        EmployeeStatus.Active,
        EffectiveDate,
        "Rehire request",
        string.Empty);
}
