using System.ComponentModel.DataAnnotations;

namespace EmployeeDirectory.Models;

public enum Department
{
    Engineering,
    Sales,
    Marketing,
    HumanResources,
    Finance,
    Support
}

public enum EmployeeStatus
{
    Active,
    OnLeave,
    Terminated
}

public class Employee
{
    public int Id { get; set; }

    [Required(ErrorMessage = "First name is required.")]
    [StringLength(50)]
    [Display(Name = "First Name")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required.")]
    [StringLength(50)]
    [Display(Name = "Last Name")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public Department Department { get; set; }

    [Required]
    [Display(Name = "Hire Date")]
    [DataType(DataType.Date)]
    public DateTime HireDate { get; set; } = DateTime.Today;

    [Required]
    public EmployeeStatus Status { get; set; } = EmployeeStatus.Active;

    [StringLength(50)]
    public string Title { get; set; } = string.Empty;

    public string FullName => $"{FirstName} {LastName}";
}
