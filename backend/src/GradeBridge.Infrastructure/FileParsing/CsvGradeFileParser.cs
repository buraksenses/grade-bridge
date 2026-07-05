using System.Globalization;
using System.Text.Json;
using CsvHelper;
using CsvHelper.Configuration;
using GradeBridge.Application.Abstractions;
using GradeBridge.Application.DTOs;
using GradeBridge.Application.Validation;

namespace GradeBridge.Infrastructure.FileParsing;

public sealed class CsvGradeFileParser : IGradeFileParser
{
    public bool CanParse(string fileName)
        => Path.GetExtension(fileName).Equals(".csv", StringComparison.OrdinalIgnoreCase);

    public Task<ParsedGradeFileResult> ParseAsync(Stream fileStream, string fileName, CancellationToken cancellationToken)
    {
        using var reader = new StreamReader(fileStream);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            DetectDelimiter = true,
            HeaderValidated = null,
            MissingFieldFound = null,
            TrimOptions = TrimOptions.Trim
        });

        var rows = new List<ParsedGradeRowDto>();
        csv.Read();
        csv.ReadHeader();

        var headers = csv.HeaderRecord ?? [];

        while (csv.Read())
        {
            cancellationToken.ThrowIfCancellationRequested();

            var dto = new ParsedGradeRowDto
            {
                RowNumber = csv.Context.Parser.Row,
                StudentNumber = ReadByPossibleHeaders(csv, headers, ["Öğrenci No", "Ogrenci No", "StudentNumber", "Numara", "No"]),
                FullName = ReadByPossibleHeaders(csv, headers, ["Ad Soyad", "Adi Soyadi", "FullName", "Name"]),
                Grade = ReadDecimalByPossibleHeaders(csv, headers, ["Not", "Puan", "Score", "Grade", "Final", "Vize"]),
                CorrectCount = ReadIntByPossibleHeaders(csv, headers, ["Doğru", "Dogru", "Correct"]),
                WrongCount = ReadIntByPossibleHeaders(csv, headers, ["Yanlış", "Yanlis", "Wrong"]),
                EmptyCount = ReadIntByPossibleHeaders(csv, headers, ["Boş", "Bos", "Empty"])
            };

            dto.RawRowJson = JsonSerializer.Serialize(dto);
            rows.Add(GradeRowValidationHelper.Validate(dto));
        }

        rows = GradeRowValidationHelper.ValidateDuplicates(rows);
        return Task.FromResult(new ParsedGradeFileResult { Rows = rows });
    }

    private static string? ReadByPossibleHeaders(CsvReader csv, string[] headers, string[] candidates)
    {
        var header = headers.FirstOrDefault(h => candidates.Any(c => Normalize(h).Contains(Normalize(c))));
        if (header is null)
            return null;

        return csv.TryGetField<string>(header, out var value) && !string.IsNullOrWhiteSpace(value)
            ? value.Trim()
            : null;
    }

    private static decimal? ReadDecimalByPossibleHeaders(CsvReader csv, string[] headers, string[] candidates)
    {
        var value = ReadByPossibleHeaders(csv, headers, candidates)?.Replace(',', '.');
        return decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsed)
            ? parsed
            : null;
    }

    private static int? ReadIntByPossibleHeaders(CsvReader csv, string[] headers, string[] candidates)
    {
        var value = ReadDecimalByPossibleHeaders(csv, headers, candidates);
        return value is null ? null : Convert.ToInt32(value.Value);
    }

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
}
