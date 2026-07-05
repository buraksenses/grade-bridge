using GradeBridge.Application.DTOs;

namespace GradeBridge.Application.Abstractions;

public interface IGradeFileParser
{
    bool CanParse(string fileName);

    Task<ParsedGradeFileResult> ParseAsync(
        Stream fileStream,
        string fileName,
        CancellationToken cancellationToken);
}
