using FluentValidation;

namespace GradeBridge.Application.Validation;

public interface IGradeValidationEngine
{
    IReadOnlyCollection<GradeValidationError> Validate(
        IReadOnlyCollection<ParsedGradeRowValidationModel> rows);
}