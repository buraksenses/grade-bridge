namespace GradeBridge.Application.DTOs;

public sealed class GradeTransferResult
{
    public bool IsSuccess { get; set; }
    public int SuccessCount { get; set; }
    public int FailedCount { get; set; }
    public string? Message { get; set; }
    public List<GradeTransferError> Errors { get; set; } = [];

    public static GradeTransferResult Success(int successCount, string? message = null)
        => new() { IsSuccess = true, SuccessCount = successCount, Message = message };

    public static GradeTransferResult Failed(string message, List<GradeTransferError>? errors = null)
        => new() { IsSuccess = false, Message = message, FailedCount = errors?.Count ?? 0, Errors = errors ?? [] };
}

public sealed class GradeTransferError
{
    public string? StudentNumber { get; set; }
    public string Message { get; set; } = default!;
}
