namespace GradeBridge.Application.Validation;

public sealed class GradeValidationError
{
    public int RowNumber { get; init; }
    public string Code { get; init; } = default!;
    public string Message { get; init; } = default!;
}