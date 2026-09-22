using EmployeeDirectory.Models;

namespace EmployeeDirectory.Services;

public static class EmploymentStatusRules
{
    public static string? Validate(
        EmployeeStatus currentStatus,
        EmployeeStatus newStatus,
        DateTime effectiveDate)
    {
        if (currentStatus == newStatus)
        {
            return "New status must be different from the current status.";
        }

        if (effectiveDate.Date > DateTime.Today)
        {
            return "Effective date cannot be in the future.";
        }

        if (currentStatus == EmployeeStatus.Terminated)
        {
            return "A terminated employee cannot transition to another status.";
        }

        return null;
    }
}
