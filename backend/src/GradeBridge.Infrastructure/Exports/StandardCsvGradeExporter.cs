using System.Text;
using GradeBridge.Domain.Imports;

namespace GradeBridge.Infrastructure.Exports;

public sealed class StandardCsvGradeExporter
{
    public byte[] Export(IEnumerable<ParsedGradeRow> rows)
    {
        var builder = new StringBuilder();
        builder.AppendLine("StudentNumber,FullName,Grade");

        foreach (var row in rows)
        {
            builder.AppendLine($"{Escape(row.StudentNumber)},{Escape(row.FullName)},{row.Grade}");
        }

        return Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(builder.ToString())).ToArray();
    }

    private static string Escape(string? value)
    {
        value ??= string.Empty;
        value = value.Replace("\"", "\"\"");
        return $"\"{value}\"";
    }
}
