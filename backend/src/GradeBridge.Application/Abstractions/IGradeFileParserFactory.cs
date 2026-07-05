namespace GradeBridge.Application.Abstractions;

public interface IGradeFileParserFactory
{
    IGradeFileParser GetParser(string fileName);
}
