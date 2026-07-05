namespace GradeBridge.Application.Validation;

public sealed class ParsedGradeRowValidationModel
{
    public int RowNumber { get; init; }
    public string? StudentNumber { get; init; }
    public string? FullName { get; init; }
    public decimal? Grade { get; init; }
}