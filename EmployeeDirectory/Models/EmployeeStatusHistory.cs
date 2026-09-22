using System.ComponentModel.DataAnnotations;

namespace EmployeeDirectory.Models;

public class EmployeeStatusHistory
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public EmployeeStatus PreviousStatus { get; set; }
    public EmployeeStatus NewStatus { get; set; }

    [DataType(DataType.Date)]
    public DateTime EffectiveDate { get; set; }

    public string Reason { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime RecordedAtUtc { get; set; }
}
