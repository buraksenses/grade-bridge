using System.Text.Json;
using ClosedXML.Excel;
using GradeBridge.Application.Abstractions;
using GradeBridge.Application.DTOs;
using GradeBridge.Application.Validation;

namespace GradeBridge.Infrastructure.FileParsing;

public sealed class ExcelGradeFileParser : IGradeFileParser
{
    private static readonly string[] StudentNumberHeaders = ["öğrenci no", "ogrenci no", "student no", "studentnumber", "numara", "no"];
    private static readonly string[] NameHeaders = ["ad soyad", "adı soyadı", "adi soyadi", "full name", "name", "isim"];
    private static readonly string[] GradeHeaders = ["not", "puan", "score", "grade", "net", "final", "vize"];
    private static readonly string[] CorrectHeaders = ["doğru", "dogru", "correct"];
    private static readonly string[] WrongHeaders = ["yanlış", "yanlis", "wrong"];
    private static readonly string[] EmptyHeaders = ["boş", "bos", "empty"];

    public bool CanParse(string fileName)
    {
        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        return ext is ".xlsx" or ".xlsm";
    }

    public Task<ParsedGradeFileResult> ParseAsync(Stream fileStream, string fileName, CancellationToken cancellationToken)
    {
        using var workbook = new XLWorkbook(fileStream);
        var worksheet = workbook.Worksheets.First();
        var usedRange = worksheet.RangeUsed();

        if (usedRange is null)
            return Task.FromResult(new ParsedGradeFileResult());

        var headerRowNumber = FindHeaderRow(usedRange);
        var headerRow = worksheet.Row(headerRowNumber);
        var lastColumn = usedRange.RangeAddress.LastAddress.ColumnNumber;
        var lastRow = usedRange.RangeAddress.LastAddress.RowNumber;

        var columns = ResolveColumns(headerRow, lastColumn);

        var rows = new List<ParsedGradeRowDto>();

        for (var rowNumber = headerRowNumber + 1; rowNumber <= lastRow; rowNumber++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var excelRow = worksheet.Row(rowNumber);
            if (excelRow.IsEmpty())
                continue;

            var dto = new ParsedGradeRowDto
            {
                RowNumber = rowNumber,
                StudentNumber = GetCellText(excelRow, columns.StudentNumberColumn),
                FullName = GetCellText(excelRow, columns.NameColumn),
                Grade = GetDecimal(excelRow, columns.GradeColumn),
                CorrectCount = GetInt(excelRow, columns.CorrectColumn),
                WrongCount = GetInt(excelRow, columns.WrongColumn),
                EmptyCount = GetInt(excelRow, columns.EmptyColumn)
            };

            dto.RawRowJson = JsonSerializer.Serialize(new
            {
                dto.RowNumber,
                dto.StudentNumber,
                dto.FullName,
                dto.Grade,
                dto.CorrectCount,
                dto.WrongCount,
                dto.EmptyCount
            });

            rows.Add(GradeRowValidationHelper.Validate(dto));
        }

        rows = GradeRowValidationHelper.ValidateDuplicates(rows);

        return Task.FromResult(new ParsedGradeFileResult { Rows = rows });
    }

    private static int FindHeaderRow(IXLRange usedRange)
    {
        foreach (var row in usedRange.Rows())
        {
            var values = row.Cells().Select(c => Normalize(c.GetString())).ToList();
            var hasStudentNumber = values.Any(v => StudentNumberHeaders.Any(h => v.Contains(Normalize(h))));
            var hasGrade = values.Any(v => GradeHeaders.Any(h => v.Contains(Normalize(h))));

            if (hasStudentNumber && hasGrade)
                return row.RowNumber();
        }

        return usedRange.RangeAddress.FirstAddress.RowNumber;
    }

    private static ResolvedColumns ResolveColumns(IXLRow headerRow, int lastColumn)
    {
        var resolved = new ResolvedColumns();

        for (var col = 1; col <= lastColumn; col++)
        {
            var header = Normalize(headerRow.Cell(col).GetString());

            if (resolved.StudentNumberColumn is null && Matches(header, StudentNumberHeaders))
                resolved.StudentNumberColumn = col;

            if (resolved.NameColumn is null && Matches(header, NameHeaders))
                resolved.NameColumn = col;

            if (resolved.GradeColumn is null && Matches(header, GradeHeaders))
                resolved.GradeColumn = col;

            if (resolved.CorrectColumn is null && Matches(header, CorrectHeaders))
                resolved.CorrectColumn = col;

            if (resolved.WrongColumn is null && Matches(header, WrongHeaders))
                resolved.WrongColumn = col;

            if (resolved.EmptyColumn is null && Matches(header, EmptyHeaders))
                resolved.EmptyColumn = col;
        }

        return resolved;
    }

    private static bool Matches(string header, IEnumerable<string> candidates)
        => candidates.Any(candidate => header.Contains(Normalize(candidate)));

    private static string Normalize(string? value)
        => (value ?? string.Empty)
            .Trim()
            .ToLowerInvariant()
            .Replace("ı", "i")
            .Replace("ğ", "g")
            .Replace("ü", "u")
            .Replace("ş", "s")
            .Replace("ö", "o")
            .Replace("ç", "c");

    private static string? GetCellText(IXLRow row, int? column)
    {
        if (column is null)
            return null;

        var value = row.Cell(column.Value).GetString()?.Trim();
        return string.IsNullOrWhiteSpace(value) ? null : value;
    }

    private static decimal? GetDecimal(IXLRow row, int? column)
    {
        if (column is null)
            return null;

        var cell = row.Cell(column.Value);

        if (cell.TryGetValue<decimal>(out var decimalValue))
            return decimalValue;

        var text = cell.GetString()?.Trim().Replace(',', '.');
        return decimal.TryParse(text, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var parsed)
            ? parsed
            : null;
    }

    private static int? GetInt(IXLRow row, int? column)
    {
        var value = GetDecimal(row, column);
        return value is null ? null : Convert.ToInt32(value.Value);
    }

    private sealed class ResolvedColumns
    {
        public int? StudentNumberColumn { get; set; }
        public int? NameColumn { get; set; }
        public int? GradeColumn { get; set; }
        public int? CorrectColumn { get; set; }
        public int? WrongColumn { get; set; }
        public int? EmptyColumn { get; set; }
    }
}
