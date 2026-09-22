using System.Collections.Concurrent;
using EmployeeDirectory.Models;

namespace EmployeeDirectory.Services;

public interface IEmployeeRepository
{
    IReadOnlyList<Employee> GetAll();
    Employee? GetById(int id);
    Employee Add(Employee employee);
    bool Update(Employee employee);
    bool Delete(int id);
    IReadOnlyList<EmployeeStatusHistory> GetStatusHistory(int employeeId);
    bool ChangeStatus(
        int employeeId,
        EmployeeStatus newStatus,
        DateTime effectiveDate,
        string reason,
        string? notes);
    void ResetToSeedData();
}

/// <summary>
/// In-memory ("temporary") storage for employees. Data lives only for the lifetime of the
/// process — perfect for demoing CRUD/search flows to an automation tool without a real database.
/// Registered as a singleton so all requests share the same in-memory state.
/// </summary>
public class InMemoryEmployeeRepository : IEmployeeRepository
{
    private readonly ConcurrentDictionary<int, Employee> _store = new();
    private readonly ConcurrentDictionary<int, List<EmployeeStatusHistory>> _statusHistory = new();
    private int _nextId;
    private int _nextHistoryId;
    private readonly object _idLock = new();
    private readonly object _lifecycleLock = new();

    public InMemoryEmployeeRepository()
    {
        ResetToSeedData();
    }

    public IReadOnlyList<Employee> GetAll() => _store.Values.OrderBy(e => e.Id).ToList();

    public Employee? GetById(int id) => _store.TryGetValue(id, out var employee) ? employee : null;

    public Employee Add(Employee employee)
    {
        lock (_idLock)
        {
            employee.Id = ++_nextId;
        }
        _store[employee.Id] = employee;
        return employee;
    }

    public bool Update(Employee employee)
    {
        if (!_store.ContainsKey(employee.Id))
        {
            return false;
        }
        _store[employee.Id] = employee;
        return true;
    }

    public bool Delete(int id)
    {
        _statusHistory.TryRemove(id, out _);
        return _store.TryRemove(id, out _);
    }

    public IReadOnlyList<EmployeeStatusHistory> GetStatusHistory(int employeeId)
    {
        lock (_lifecycleLock)
        {
            return _statusHistory.TryGetValue(employeeId, out var history)
                ? history
                    .OrderByDescending(item => item.EffectiveDate)
                    .ThenByDescending(item => item.RecordedAtUtc)
                    .ToList()
                : [];
        }
    }

    public bool ChangeStatus(
        int employeeId,
        EmployeeStatus newStatus,
        DateTime effectiveDate,
        string reason,
        string? notes)
    {
        lock (_lifecycleLock)
        {
            if (!_store.TryGetValue(employeeId, out var employee))
            {
                return false;
            }

            var history = _statusHistory.GetOrAdd(employeeId, _ => []);
            history.Add(new EmployeeStatusHistory
            {
                Id = ++_nextHistoryId,
                EmployeeId = employeeId,
                PreviousStatus = employee.Status,
                NewStatus = newStatus,
                EffectiveDate = effectiveDate.Date,
                Reason = reason.Trim(),
                Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim(),
                RecordedAtUtc = DateTime.UtcNow
            });

            employee.Status = newStatus;
            return true;
        }
    }

    /// <summary>
    /// Restores the repository to its original seed data. Exposed so an automation
    /// test tool can reset state between test runs without restarting the app.
    /// </summary>
    public void ResetToSeedData()
    {
        _store.Clear();
        lock (_lifecycleLock)
        {
            _statusHistory.Clear();
            _nextHistoryId = 0;
        }
        lock (_idLock)
        {
            _nextId = 0;
        }

        var seed = new List<Employee>
        {
            new() { FirstName = "Ava", LastName = "Thompson", Email = "ava.thompson@example.com", Department = Department.Engineering, Title = "Senior Software Engineer", HireDate = new DateTime(2019, 3, 14), Status = EmployeeStatus.Active },
            new() { FirstName = "Liam", LastName = "Garcia", Email = "liam.garcia@example.com", Department = Department.Sales, Title = "Account Executive", HireDate = new DateTime(2021, 7, 1), Status = EmployeeStatus.Active },
            new() { FirstName = "Noah", LastName = "Patel", Email = "noah.patel@example.com", Department = Department.Marketing, Title = "Marketing Specialist", HireDate = new DateTime(2020, 11, 23), Status = EmployeeStatus.OnLeave },
            new() { FirstName = "Emma", LastName = "Nguyen", Email = "emma.nguyen@example.com", Department = Department.HumanResources, Title = "HR Business Partner", HireDate = new DateTime(2018, 5, 9), Status = EmployeeStatus.Active },
            new() { FirstName = "Oliver", LastName = "Kim", Email = "oliver.kim@example.com", Department = Department.Finance, Title = "Financial Analyst", HireDate = new DateTime(2022, 1, 17), Status = EmployeeStatus.Active },
            new() { FirstName = "Sophia", LastName = "Rossi", Email = "sophia.rossi@example.com", Department = Department.Support, Title = "Customer Support Lead", HireDate = new DateTime(2017, 9, 2), Status = EmployeeStatus.Terminated },
            new() { FirstName = "Ethan", LastName = "Brown", Email = "ethan.brown@example.com", Department = Department.Engineering, Title = "QA Engineer", HireDate = new DateTime(2023, 2, 6), Status = EmployeeStatus.Active },
            new() { FirstName = "Mia", LastName = "Davies", Email = "mia.davies@example.com", Department = Department.Sales, Title = "Sales Manager", HireDate = new DateTime(2016, 10, 30), Status = EmployeeStatus.Active },
        };

        foreach (var employee in seed)
        {
            Add(employee);
        }
    }
}
