using GradeBridge.Domain.Common;

namespace GradeBridge.Domain.Imports;

public sealed class ImportJob : BaseEntity
{
    public Guid InstitutionId { get; set; }
    public Guid? UploadedByUserId { get; set; }

    public string CourseCode { get; set; } = default!;
    public string ExamCode { get; set; } = default!;

    public string OriginalFileName { get; set; } = default!;
    public string? StoredFilePath { get; set; }

    public ImportJobStatus Status { get; set; } = ImportJobStatus.Uploaded;
    public string? ErrorMessage { get; set; }

    public List<ParsedGradeRow> Rows { get; set; } = [];
}
