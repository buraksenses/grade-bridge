namespace GradeBridge.Application.Validation;

public interface IGradeValidationRule
{
    string Code { get; }

    GradeValidationError? Validate(
        ParsedGradeRowValidationModel row,
        GradeValidationContext context);
}