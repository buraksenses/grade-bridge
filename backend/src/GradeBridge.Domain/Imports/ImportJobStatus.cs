namespace GradeBridge.Domain.Imports;

public enum ImportJobStatus
{
    Uploaded = 1,
    Parsed = 2,
    HasValidationErrors = 3,
    ReadyForApproval = 4,
    Approved = 5,
    Exported = 6,
    TransferSimulated = 7,
    Failed = 8
}
