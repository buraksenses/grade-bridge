using GradeBridge.Application.Abstractions;
using GradeBridge.Infrastructure.Exports;
using GradeBridge.Infrastructure.FileParsing;
using GradeBridge.Infrastructure.Integrations;
using GradeBridge.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GradeBridge.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<GradeBridgeDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        });

        services.AddScoped<IGradeFileParser, ExcelGradeFileParser>();
        services.AddScoped<IGradeFileParser, CsvGradeFileParser>();
        services.AddScoped<IGradeFileParserFactory, GradeFileParserFactory>();

        services.AddScoped<IGradeTransferAdapter, MockGradeTransferAdapter>();
        services.AddScoped<StandardCsvGradeExporter>();

        return services;
    }
}
