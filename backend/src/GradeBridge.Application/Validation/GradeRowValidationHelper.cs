using GradeBridge.Application.DTOs;

namespace GradeBridge.Application.Validation;

public static class GradeRowValidationHelper
{
    public static ParsedGradeRowDto Validate(ParsedGradeRowDto row)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(row.StudentNumber))
            errors.Add("Öğrenci numarası boş.");

        if (row.Grade is null)
            errors.Add("Not değeri okunamadı.");
        else if (row.Grade < 0 || row.Grade > 100)
            errors.Add("Not değeri 0-100 aralığında olmalı.");

        row.IsValid = errors.Count == 0;
        row.ErrorMessage = errors.Count == 0 ? null : string.Join(" ", errors);

        return row;
    }

    public static List<ParsedGradeRowDto> ValidateDuplicates(List<ParsedGradeRowDto> rows)
    {
        var duplicateStudentNumbers = rows
            .Where(x => !string.IsNullOrWhiteSpace(x.StudentNumber))
            .GroupBy(x => x.StudentNumber!.Trim())
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var row in rows)
        {
            if (!string.IsNullOrWhiteSpace(row.StudentNumber) && duplicateStudentNumbers.Contains(row.StudentNumber.Trim()))
            {
                row.IsValid = false;
                row.ErrorMessage = string.IsNullOrWhiteSpace(row.ErrorMessage)
                    ? "Aynı öğrenci numarası dosyada birden fazla kez bulunmuş."
                    : row.ErrorMessage + " Aynı öğrenci numarası dosyada birden fazla kez bulunmuş.";
            }
        }

        return rows;
    }
}
