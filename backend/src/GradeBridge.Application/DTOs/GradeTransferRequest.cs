namespace GradeBridge.Application.DTOs;

public sealed class GradeTransferRequest
{
    public Guid InstitutionId { get; set; }
    public Guid ImportJobId { get; set; }
    public string CourseCode { get; set; } = default!;
    public string ExamCode { get; set; } = default!;
    public List<GradeTransferRow> Grades { get; set; } = [];
}

public sealed class GradeTransferRow
{
    public string StudentNumber { get; set; } = default!;
    public string? FullName { get; set; }
    public decimal Grade { get; set; }
}
