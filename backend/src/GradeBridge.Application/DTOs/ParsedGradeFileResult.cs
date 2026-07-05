namespace GradeBridge.Application.DTOs;

public sealed class ParsedGradeFileResult
{
    public List<ParsedGradeRowDto> Rows { get; set; } = [];
    public int TotalRows => Rows.Count;
    public int ValidRows => Rows.Count(x => x.IsValid);
    public int InvalidRows => Rows.Count(x => !x.IsValid);
}
