using EmployeeDirectory.Models;
using EmployeeDirectory.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeDirectory.Controllers;

public class EmployeesController : Controller
{
    private readonly IEmployeeRepository _repository;

    public EmployeesController(IEmployeeRepository repository)
    {
        _repository = repository;
    }

    // GET: /Employees  (search + list page)
    [HttpGet]
    public IActionResult Index(string? searchTerm, Department? department, EmployeeStatus? status)
    {
        var query = _repository.GetAll().AsEnumerable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            query = query.Where(e =>
                e.FirstName.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                e.LastName.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                e.Email.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                e.Title.Contains(term, StringComparison.OrdinalIgnoreCase));
        }

        if (department.HasValue)
        {
            query = query.Where(e => e.Department == department.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(e => e.Status == status.Value);
        }

        var results = query.OrderBy(e => e.LastName).ThenBy(e => e.FirstName).ToList();

        var viewModel = new EmployeeSearchViewModel
        {
            SearchTerm = searchTerm,
            Department = department,
            Status = status,
            Results = results,
            TotalCount = results.Count
        };

        return View(viewModel);
    }

    // GET: /Employees/Details/5
    [HttpGet]
    public IActionResult Details(int id)
    {
        var employee = _repository.GetById(id);
        if (employee == null)
        {
            return NotFound();
        }

        return View(new EmployeeDetailsViewModel
        {
            Employee = employee,
            StatusHistory = _repository.GetStatusHistory(id)
        });
    }

    // GET: /Employees/Create
    [HttpGet]
    public IActionResult Create()
    {
        return View(new Employee { HireDate = DateTime.Today });
    }

    // POST: /Employees/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Employee employee)
    {
        if (!ModelState.IsValid)
        {
            return View(employee);
        }

        _repository.Add(employee);
        TempData["StatusMessage"] = $"Employee \"{employee.FullName}\" was created successfully.";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Employees/Edit/5
    [HttpGet]
    public IActionResult Edit(int id)
    {
        var employee = _repository.GetById(id);
        if (employee == null)
        {
            return NotFound();
        }
        return View(employee);
    }

    // POST: /Employees/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Employee employee)
    {
        if (id != employee.Id)
        {
            return BadRequest();
        }

        var currentEmployee = _repository.GetById(id);
        if (currentEmployee == null)
        {
            return NotFound();
        }

        employee.Status = currentEmployee.Status;

        if (!ModelState.IsValid)
        {
            return View(employee);
        }

        var updated = _repository.Update(employee);
        if (!updated)
        {
            return NotFound();
        }

        TempData["StatusMessage"] = $"Employee \"{employee.FullName}\" was updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult ChangeStatus(int id)
    {
        var employee = _repository.GetById(id);
        if (employee == null)
        {
            return NotFound();
        }

        return View(new ChangeEmployeeStatusViewModel
        {
            EmployeeId = employee.Id,
            EmployeeName = employee.FullName,
            CurrentStatus = employee.Status,
            NewStatus = employee.Status == EmployeeStatus.Active
                ? EmployeeStatus.OnLeave
                : EmployeeStatus.Active,
            EffectiveDate = DateTime.Today
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ChangeStatus(ChangeEmployeeStatusViewModel model)
    {
        var employee = _repository.GetById(model.EmployeeId);
        if (employee == null)
        {
            return NotFound();
        }

        model.EmployeeName = employee.FullName;
        model.CurrentStatus = employee.Status;
        ModelState.Remove(nameof(model.EmployeeName));
        ModelState.Remove(nameof(model.CurrentStatus));

        var ruleError = EmploymentStatusRules.Validate(
            employee.Status,
            model.NewStatus,
            model.EffectiveDate);
        if (ruleError is not null)
        {
            ModelState.AddModelError(nameof(model.NewStatus), ruleError);
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (!_repository.ChangeStatus(
                model.EmployeeId,
                model.NewStatus,
                model.EffectiveDate,
                model.Reason,
                model.Notes))
        {
            return NotFound();
        }

        TempData["StatusMessage"] =
            $"Status for \"{employee.FullName}\" changed from {model.CurrentStatus} to {model.NewStatus}.";
        return RedirectToAction(nameof(Details), new { id = model.EmployeeId });
    }

    // GET: /Employees/Delete/5
    [HttpGet]
    public IActionResult Delete(int id)
    {
        var employee = _repository.GetById(id);
        if (employee == null)
        {
            return NotFound();
        }
        return View(employee);
    }

    // POST: /Employees/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var employee = _repository.GetById(id);
        _repository.Delete(id);
        TempData["StatusMessage"] = employee != null
            ? $"Employee \"{employee.FullName}\" was deleted."
            : "Employee was deleted.";
        return RedirectToAction(nameof(Index));
    }

    // POST: /Employees/Reset  (helper for automation test tools to restore known state)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Reset()
    {
        _repository.ResetToSeedData();
        TempData["StatusMessage"] = "Data was reset to the seeded sample set.";
        return RedirectToAction(nameof(Index));
    }
}
