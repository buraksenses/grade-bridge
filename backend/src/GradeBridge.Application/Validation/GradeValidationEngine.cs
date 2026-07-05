using FluentValidation;

namespace GradeBridge.Application.Validation;

public sealed class GradeValidationEngine : IGradeValidationEngine
{
    private readonly IValidator<ParsedGradeRowValidationModel> _rowValidator;

    public GradeValidationEngine(
        IValidator<ParsedGradeRowValidationModel> rowValidator)
    {
        _rowValidator = rowValidator;
    }

    public IReadOnlyCollection<GradeValidationError> Validate(
        IReadOnlyCollection<ParsedGradeRowValidationModel> rows)
    {
        var errors = new List<GradeValidationError>();

        foreach (var row in rows)
        {
            var result = _rowValidator.Validate(row);

            errors.AddRange(result.Errors.Select(error => new GradeValidationError
            {
                RowNumber = row.RowNumber,
                Code = error.ErrorCode,
                Message = error.ErrorMessage
            }));
        }

        AddDuplicateStudentNumberErrors(rows, errors);

        return errors;
    }

    private static void AddDuplicateStudentNumberErrors(
        IReadOnlyCollection<ParsedGradeRowValidationModel> rows,
        List<GradeValidationError> errors)
    {
        var duplicateNumbers = rows
            .Where(x => !string.IsNullOrWhiteSpace(x.StudentNumber))
            .GroupBy(x => x.StudentNumber!.Trim(), StringComparer.OrdinalIgnoreCase)
            .Where(x => x.Count() > 1)
            .Select(x => x.Key)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var row in rows.Where(x =>
                     !string.IsNullOrWhiteSpace(x.StudentNumber) &&
                     duplicateNumbers.Contains(x.StudentNumber.Trim())))
        {
            errors.Add(new GradeValidationError
            {
                RowNumber = row.RowNumber,
                Code = "DUPLICATE_STUDENT_NUMBER",
                Message = $"Öğrenci numarası tekrar ediyor: {row.StudentNumber}"
            });
        }
    }
}