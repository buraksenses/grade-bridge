# GradeBridge Architecture - 04 Adapter Architecture

## Purpose

GradeBridge must support different institutions, optical reader outputs, APIs, and export formats without rewriting the core application.

## Core Principle

> Core stays the same. Adapters change.

Core does not know external systems. Adapters know external systems.

## Adapter Types

### Input Adapters / Parsers

Read external files and convert them into normalized internal rows.

### Export Adapters

Convert approved grades into downloadable files such as CSV, Excel, or UBYS-specific templates.

### Transfer Adapters

Send approved grades to external systems such as UBYS, OBS, or mock targets.

## Transfer Adapter Interface

```csharp
public interface IGradeTransferAdapter
{
    Task<GradeTransferResult> TransferAsync(
        GradeTransferRequest request,
        CancellationToken cancellationToken);
}
```

## Adapter Factory

The adapter factory chooses the correct adapter for each institution.

```csharp
public interface IGradeTransferAdapterFactory
{
    IGradeTransferAdapter Create(string integrationType);
}
```

Example:

```csharp
public class GradeTransferAdapterFactory : IGradeTransferAdapterFactory
{
    private readonly IServiceProvider _serviceProvider;

    public IGradeTransferAdapter Create(string integrationType)
    {
        return integrationType switch
        {
            "Mock" => _serviceProvider.GetRequiredService<MockGradeTransferAdapter>(),
            "TrabzonUbys" => _serviceProvider.GetRequiredService<TrabzonUbysGradeTransferAdapter>(),
            "ExcelExport" => _serviceProvider.GetRequiredService<ExcelExportTransferAdapter>(),
            _ => throw new NotSupportedException($"Adapter not supported: {integrationType}")
        };
    }
}
```

## Why This Matters

Without adapters the code turns into institution-specific if/else blocks. With adapters, new institutions are added safely and the core remains stable.
