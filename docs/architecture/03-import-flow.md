# GradeBridge Architecture - 03 Import Flow

## Purpose

This document explains the full import lifecycle from file upload to export or transfer.

## Import Lifecycle

```text
1. Lecturer uploads file
2. API creates ImportJob
3. Parser adapter reads file
4. Parsed rows are normalized
5. Validation engine runs
6. Rows are persisted
7. Lecturer reviews rows
8. Lecturer approves valid rows
9. System exports or transfers grades
10. Transfer/export result is logged
```

## Upload

Endpoint example:

```http
POST /api/import-jobs/upload
Content-Type: multipart/form-data
```

Form data:

```text
file: results.csv
courseCode: MAT101
examCode: FINAL
```

## Parser Selection

ParserFactory decides which parser to use:

```text
.csv  → CsvGradeFileParser
.xlsx → ExcelGradeFileParser
.docx → WordGradeFileParser
```

## Normalization

Every parser returns the same internal model:

```csharp
public class ParsedGradeRowDto
{
    public int RowNumber { get; set; }
    public string? StudentNumber { get; set; }
    public string? FullName { get; set; }
    public decimal? Grade { get; set; }
}
```

## Validation

Checks include required student number, required grade, grade range, duplicate student numbers, missing full name if needed, and valid course/exam codes.

## Review and Approval

The lecturer sees a table of parsed rows and approves valid rows. Approval means rows are ready for export or transfer. It does not mean they were already sent to an external system.

## Export / Transfer

Export generates CSV, Excel, or an external system-specific template. Transfer sends approved grades to mock adapter, UBYS API, OBS API, or another academic system.
