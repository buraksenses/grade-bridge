namespace GradeBridge.Application.Validation;

public sealed class GradeValidationContext
{
    public IReadOnlyCollection<ParsedGradeRowValidationModel> AllRows { get; init; }
        = new List<ParsedGradeRowValidationModel>();
}