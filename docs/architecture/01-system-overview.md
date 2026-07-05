# GradeBridge Architecture - 01 System Overview

## Purpose

GradeBridge is a grade import and transfer platform. It takes exam result files such as optical reader Excel, CSV, or Word outputs, parses them into a normalized grade model, validates records, allows lecturer approval, and then exports or transfers grades to external academic systems such as UBYS or OBS.

Core principle:

> Core business logic must not depend on a specific university, OBS system, optical reader format, or API.

External systems are handled through adapters.

## High Level Flow

```text
Lecturer
↓
React Web App
↓
GradeBridge .NET API
↓
Core Grade Engine
↓
Parser / Validation / Review / Export / Transfer
↓
SQL Server + File Storage
↓
External UBYS / OBS / Excel Export / Mock Adapter
```

## Main Components

### React Web App

Responsible for login, file upload, import job list, parsed grade review, validation errors, lecturer approval, export or transfer trigger, and transfer result display. The frontend must never call external UBYS/OBS APIs directly.

### GradeBridge API

The .NET API is the main entry point. It handles authentication, authorization, file upload, orchestration, parser calls, validation, persistence, export/transfer adapter calls, and audit logging.

### Core Grade Engine

Contains system-independent business logic: normalize parsed grade data, run validation rules, prepare review data, approve/reject rows, prepare transfer request, and record transfer result.

### Parser Adapters

Convert external file formats into internal normalized rows. Examples: CsvGradeFileParser, ExcelGradeFileParser, WordGradeFileParser.

### Transfer Adapters

Convert internal approved grades into external API requests or export formats. Examples: MockGradeTransferAdapter, StandardCsvExportAdapter, TrabzonUbysGradeTransferAdapter.

## Current MVP Scope

Included: CSV upload, Excel upload, parse rows, validate grade values, persist import job, review rows, approve import, export CSV, mock transfer adapter.

Excluded: Real UBYS API integration, browser automation/RPA, SMS login automation, direct live grade submission.
