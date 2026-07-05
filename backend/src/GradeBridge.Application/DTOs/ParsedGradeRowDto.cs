namespace GradeBridge.Application.DTOs;

public sealed class ParsedGradeRowDto
{
    public int RowNumber { get; set; }
    public string? StudentNumber { get; set; }
    public string? FullName { get; set; }
    public decimal? Grade { get; set; }
    public int? CorrectCount { get; set; }
    public int? WrongCount { get; set; }
    public int? EmptyCount { get; set; }
    public bool IsValid { get; set; }
    public string? ErrorMessage { get; set; }
    public string? RawRowJson { get; set; }
}
