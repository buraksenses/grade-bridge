using GradeBridge.Application.Abstractions;
using GradeBridge.Application.DTOs;
using GradeBridge.Domain.Imports;
using GradeBridge.Infrastructure.Exports;
using GradeBridge.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GradeBridge.Api.Controllers;

[ApiController]
[Route("api/import-jobs")]
public sealed class ImportJobsController : ControllerBase
{
    private readonly GradeBridgeDbContext _dbContext;
    private readonly IGradeFileParserFactory _parserFactory;
    private readonly IEnumerable<IGradeTransferAdapter> _transferAdapters;
    private readonly StandardCsvGradeExporter _csvExporter;

    public ImportJobsController(
        GradeBridgeDbContext dbContext,
        IGradeFileParserFactory parserFactory,
        IEnumerable<IGradeTransferAdapter> transferAdapters,
        StandardCsvGradeExporter csvExporter)
    {
        _dbContext = dbContext;
        _parserFactory = parserFactory;
        _transferAdapters = transferAdapters;
        _csvExporter = csvExporter;
    }

    [HttpPost("upload")]
    [RequestSizeLimit(25_000_000)]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Upload(
        IFormFile file,
        [FromForm] string courseCode,
        [FromForm] string examCode,
        [FromForm] Guid? institutionId,
        CancellationToken cancellationToken)
    {
        if (file.Length == 0)
            return BadRequest("Dosya boş.");

        var parser = _parserFactory.GetParser(file.FileName);

        await using var stream = file.OpenReadStream();
        var parsed = await parser.ParseAsync(stream, file.FileName, cancellationToken);

        var job = new ImportJob
        {
            InstitutionId = institutionId ?? Guid.Empty,
            CourseCode = courseCode,
            ExamCode = examCode,
            OriginalFileName = file.FileName,
            Status = parsed.InvalidRows > 0 ? ImportJobStatus.HasValidationErrors : ImportJobStatus.ReadyForApproval,
            Rows = parsed.Rows.Select(row => new ParsedGradeRow
            {
                RowNumber = row.RowNumber,
                StudentNumber = row.StudentNumber,
                FullName = row.FullName,
                Grade = row.Grade,
                CorrectCount = row.CorrectCount,
                WrongCount = row.WrongCount,
                EmptyCount = row.EmptyCount,
                IsValid = row.IsValid,
                IsApproved = false,
                ErrorMessage = row.ErrorMessage,
                RawRowJson = row.RawRowJson
            }).ToList()
        };

        _dbContext.ImportJobs.Add(job);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Ok(new
        {
            job.Id,
            job.Status,
            parsed.TotalRows,
            parsed.ValidRows,
            parsed.InvalidRows
        });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
    {
        var job = await _dbContext.ImportJobs
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new
            {
                x.Id,
                x.InstitutionId,
                x.CourseCode,
                x.ExamCode,
                x.OriginalFileName,
                x.Status,
                x.CreatedAtUtc,
                TotalRows = x.Rows.Count,
                ValidRows = x.Rows.Count(r => r.IsValid),
                InvalidRows = x.Rows.Count(r => !r.IsValid),
                ApprovedRows = x.Rows.Count(r => r.IsApproved)
            })
            .FirstOrDefaultAsync(cancellationToken);

        return job is null ? NotFound() : Ok(job);
    }

    [HttpGet("{id:guid}/rows")]
    public async Task<IActionResult> GetRows(Guid id, CancellationToken cancellationToken)
    {
        var rows = await _dbContext.ParsedGradeRows
            .AsNoTracking()
            .Where(x => x.ImportJobId == id)
            .OrderBy(x => x.RowNumber)
            .Select(x => new
            {
                x.Id,
                x.RowNumber,
                x.StudentNumber,
                x.FullName,
                x.Grade,
                x.CorrectCount,
                x.WrongCount,
                x.EmptyCount,
                x.IsValid,
                x.IsApproved,
                x.ErrorMessage
            })
            .ToListAsync(cancellationToken);

        return Ok(rows);
    }

    [HttpPost("{id:guid}/approve")]
    public async Task<IActionResult> Approve(Guid id, CancellationToken cancellationToken)
    {
        var job = await _dbContext.ImportJobs
            .Include(x => x.Rows)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (job is null)
            return NotFound();

        if (job.Rows.Any(x => !x.IsValid))
            return BadRequest("Geçersiz satırlar varken onay verilemez.");

        foreach (var row in job.Rows)
        {
            row.IsApproved = true;
            row.UpdatedAtUtc = DateTime.UtcNow;
        }

        job.Status = ImportJobStatus.Approved;
        job.UpdatedAtUtc = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Ok(new { job.Id, job.Status, ApprovedRows = job.Rows.Count });
    }

    [HttpGet("{id:guid}/export/csv")]
    public async Task<IActionResult> ExportCsv(Guid id, CancellationToken cancellationToken)
    {
        var job = await _dbContext.ImportJobs
            .Include(x => x.Rows)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (job is null)
            return NotFound();

        var approvedRows = job.Rows
            .Where(x => x.IsValid && x.IsApproved)
            .OrderBy(x => x.RowNumber)
            .ToList();

        if (approvedRows.Count == 0)
            return BadRequest("Export için onaylanmış satır yok.");

        var bytes = _csvExporter.Export(approvedRows);
        var fileName = $"gradebridge-{job.CourseCode}-{job.ExamCode}.csv";

        job.Status = ImportJobStatus.Exported;
        job.UpdatedAtUtc = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return File(bytes, "text/csv", fileName);
    }

    [HttpPost("{id:guid}/transfer/mock")]
    public async Task<IActionResult> TransferMock(Guid id, CancellationToken cancellationToken)
    {
        var job = await _dbContext.ImportJobs
            .Include(x => x.Rows)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (job is null)
            return NotFound();

        var rows = job.Rows.Where(x => x.IsValid && x.IsApproved).ToList();

        if (rows.Count == 0)
            return BadRequest("Aktarım için onaylanmış satır yok.");

        var adapter = _transferAdapters.First(x => x.AdapterKey == "Mock");
        var request = new GradeTransferRequest
        {
            InstitutionId = job.InstitutionId,
            ImportJobId = job.Id,
            CourseCode = job.CourseCode,
            ExamCode = job.ExamCode,
            Grades = rows.Select(x => new GradeTransferRow
            {
                StudentNumber = x.StudentNumber!,
                FullName = x.FullName,
                Grade = x.Grade!.Value
            }).ToList()
        };

        var result = await adapter.TransferAsync(request, cancellationToken);

        job.Status = ImportJobStatus.TransferSimulated;
        job.UpdatedAtUtc = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Ok(result);
    }
}
