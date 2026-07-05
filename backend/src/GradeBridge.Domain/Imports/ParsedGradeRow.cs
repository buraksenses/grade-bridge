using GradeBridge.Domain.Common;

namespace GradeBridge.Domain.Imports;

public sealed class ParsedGradeRow : BaseEntity
{
    public Guid ImportJobId { get; set; }
    public ImportJob ImportJob { get; set; } = default!;

    public int RowNumber { get; set; }

    public string? StudentNumber { get; set; }
    public string? FullName { get; set; }
    public decimal? Grade { get; set; }

    public int? CorrectCount { get; set; }
    public int? WrongCount { get; set; }
    public int? EmptyCount { get; set; }

    public bool IsValid { get; set; }
    public bool IsApproved { get; set; }

    public string? ErrorMessage { get; set; }
    public string? RawRowJson { get; set; }
}
