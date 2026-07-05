using GradeBridge.Domain.Imports;
using GradeBridge.Domain.Institutions;
using Microsoft.EntityFrameworkCore;

namespace GradeBridge.Infrastructure.Persistence;

public sealed class GradeBridgeDbContext : DbContext
{
    public GradeBridgeDbContext(DbContextOptions<GradeBridgeDbContext> options) : base(options)
    {
    }

    public DbSet<Institution> Institutions => Set<Institution>();
    public DbSet<ImportJob> ImportJobs => Set<ImportJob>();
    public DbSet<ParsedGradeRow> ParsedGradeRows => Set<ParsedGradeRow>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Institution>(entity =>
        {
            entity.ToTable("Institutions");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(250).IsRequired();
            entity.Property(x => x.Code).HasMaxLength(100);
            entity.Property(x => x.IntegrationType).HasMaxLength(100).IsRequired();
        });

        modelBuilder.Entity<ImportJob>(entity =>
        {
            entity.ToTable("ImportJobs");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.CourseCode).HasMaxLength(100).IsRequired();
            entity.Property(x => x.ExamCode).HasMaxLength(100).IsRequired();
            entity.Property(x => x.OriginalFileName).HasMaxLength(500).IsRequired();
            entity.Property(x => x.StoredFilePath).HasMaxLength(1000);
            entity.Property(x => x.Status).HasConversion<int>();
            entity.Property(x => x.ErrorMessage).HasMaxLength(2000);

            entity.HasMany(x => x.Rows)
                .WithOne(x => x.ImportJob)
                .HasForeignKey(x => x.ImportJobId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ParsedGradeRow>(entity =>
        {
            entity.ToTable("ParsedGradeRows");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.StudentNumber).HasMaxLength(100);
            entity.Property(x => x.FullName).HasMaxLength(300);
            entity.Property(x => x.Grade).HasPrecision(5, 2);
            entity.Property(x => x.ErrorMessage).HasMaxLength(2000);
        });
    }
}
