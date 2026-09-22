using EmployeeDirectory.EngineeredTests.Models;
using EmployeeDirectory.EngineeredTests.Roles;

namespace EmployeeDirectory.EngineeredTests.Rules;

public static class EmployeeBusinessRules
{
    public static void EnsureCanSearch(DirectoryActor actor)
    {
        if (!actor.CanSearchEmployees)
        {
            throw new InvalidOperationException($"{actor.Name} cannot search employees.");
        }
    }

    public static void EnsureCanCreate(DirectoryActor actor, EmployeeTestData employee)
    {
        if (!actor.CanCreateEmployees)
        {
            throw new InvalidOperationException($"{actor.Name} cannot create employees.");
        }

        if (string.IsNullOrWhiteSpace(employee.Email) || !employee.Email.Contains('@'))
        {
            throw new InvalidOperationException("Employee test data must contain a valid email address.");
        }
    }

    public static void EnsureCanChangeStatus(DirectoryActor actor)
    {
        if (!actor.CanChangeEmploymentStatus)
        {
            throw new InvalidOperationException(
                $"{actor.Name} cannot change employment status.");
        }
    }
}
