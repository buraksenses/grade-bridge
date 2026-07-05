using GradeBridge.Application.Abstractions;
using GradeBridge.Application.DTOs;

namespace GradeBridge.Infrastructure.Integrations;

public sealed class MockGradeTransferAdapter : IGradeTransferAdapter
{
    public string AdapterKey => "Mock";

    public Task<GradeTransferResult> TransferAsync(GradeTransferRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(GradeTransferResult.Success(
            request.Grades.Count,
            "Mock aktarım başarılı. Gerçek ÜBYS/OBS entegrasyonu henüz çalıştırılmadı."));
    }
}
