using GradeBridge.Application.Abstractions;

namespace GradeBridge.Infrastructure.FileParsing;

public sealed class GradeFileParserFactory : IGradeFileParserFactory
{
    private readonly IEnumerable<IGradeFileParser> _parsers;

    public GradeFileParserFactory(IEnumerable<IGradeFileParser> parsers)
    {
        _parsers = parsers;
    }

    public IGradeFileParser GetParser(string fileName)
    {
        var parser = _parsers.FirstOrDefault(x => x.CanParse(fileName));

        return parser ?? throw new NotSupportedException($"Bu dosya tipi desteklenmiyor: {Path.GetExtension(fileName)}");
    }
}
