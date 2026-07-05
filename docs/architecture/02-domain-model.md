# GradeBridge Architecture - 02 Domain Model

## Purpose

This document defines the first version of the GradeBridge domain model. The model must represent the business process independently from any university system.

## Core Entities

### Institution

Represents a customer organization such as a university, faculty, department, private school, or course center.

```csharp
public class Institution
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Code { get; set; }
    public string? IntegrationType { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

### ImportJob

Represents one uploaded grade file and its processing lifecycle.

```csharp
public class ImportJob
{
    public Guid Id { get; set; }
    public Guid InstitutionId { get; set; }
    public Guid UploadedByUserId { get; set; }
    public string CourseCode { get; set; } = default!;
    public string ExamCode { get; set; } = default!;
    public string OriginalFileName { get; set; } = default!;
    public string? StoredFilePath { get; set; }
    public ImportJobStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ParsedAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime? TransferredAt { get; set; }
    public List<ParsedGradeRow> Rows { get; set; } = new();
}
```

### ParsedGradeRow

Represents a single row extracted from the uploaded file.

```csharp
public class ParsedGradeRow
{
    public Guid Id { get; set; }
    public Guid ImportJobId { get; set; }
    public int RowNumber { get; set; }
    public string? StudentNumber { get; set; }
    public string? FullName { get; set; }
    public decimal? Grade { get; set; }
    public int? CorrectCount { get; set; }
    public int? WrongCount { get; set; }
    public int? EmptyCount { get; set; }
    public bool IsValid { get; set; }
    public bool IsApproved { get; set; }
    public string? ErrorMessage { get; set; }
    public string? RawRowJson { get; set; }
}
```

## Domain Rules

1. An import job cannot be transferred before approval.
2. Rows with validation errors cannot be approved unless corrected or excluded.
3. A transfer must produce a result report.
4. External system errors must be mapped to internal transfer errors.
5. Core domain model must remain independent of external APIs.
