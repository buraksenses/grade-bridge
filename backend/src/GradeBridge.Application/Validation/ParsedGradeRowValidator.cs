using FluentValidation;

namespace GradeBridge.Application.Validation;

public sealed class ParsedGradeRowValidator 
    : AbstractValidator<ParsedGradeRowValidationModel>
{
    public ParsedGradeRowValidator()
    {
        RuleFor(x => x.StudentNumber)
            .NotEmpty()
            .WithErrorCode("MISSING_STUDENT_NUMBER")
            .WithMessage("Öğrenci numarası boş olamaz.");

        RuleFor(x => x.Grade)
            .NotNull()
            .WithErrorCode("MISSING_GRADE")
            .WithMessage("Not değeri boş olamaz.");

        RuleFor(x => x.Grade)
            .InclusiveBetween(0, 100)
            .When(x => x.Grade.HasValue)
            .WithErrorCode("INVALID_GRADE_RANGE")
            .WithMessage("Not değeri 0 ile 100 arasında olmalıdır.");
    }
}