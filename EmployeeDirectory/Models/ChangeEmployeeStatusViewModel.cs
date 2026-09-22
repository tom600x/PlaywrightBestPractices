using System.ComponentModel.DataAnnotations;

namespace EmployeeDirectory.Models;

public class ChangeEmployeeStatusViewModel
{
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public EmployeeStatus CurrentStatus { get; set; }

    [Display(Name = "New Status")]
    public EmployeeStatus NewStatus { get; set; }

    [Required(ErrorMessage = "Effective date is required.")]
    [DataType(DataType.Date)]
    [Display(Name = "Effective Date")]
    public DateTime EffectiveDate { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "A reason is required for a status change.")]
    [StringLength(200)]
    public string Reason { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Notes { get; set; }
}
