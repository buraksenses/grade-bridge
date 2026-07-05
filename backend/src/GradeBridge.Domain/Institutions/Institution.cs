using GradeBridge.Domain.Common;

namespace GradeBridge.Domain.Institutions;

public sealed class Institution : BaseEntity
{
    public string Name { get; set; } = default!;
    public string? Code { get; set; }
    public string IntegrationType { get; set; } = "StandardExcel";
    public bool IsActive { get; set; } = true;
}
