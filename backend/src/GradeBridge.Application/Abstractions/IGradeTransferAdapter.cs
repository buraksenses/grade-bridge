using GradeBridge.Application.DTOs;

namespace GradeBridge.Application.Abstractions;

public interface IGradeTransferAdapter
{
    string AdapterKey { get; }

    Task<GradeTransferResult> TransferAsync(
        GradeTransferRequest request,
        CancellationToken cancellationToken);
}
