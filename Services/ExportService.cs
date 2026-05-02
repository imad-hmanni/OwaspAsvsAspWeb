using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using WebApplicationAsp.ViewModels;

namespace WebApplicationAsp.Services
{
    public class ExportService : IExportService
    {
        private readonly IReportService _reportService;

        public ExportService(IReportService reportService) => _reportService = reportService;

        public async Task<byte[]> ExportToCsvAsync(int applicationId)
        {
            var report = await _reportService.GenerateReportAsync(applicationId);

            using var ms = new MemoryStream();
            using var writer = new StreamWriter(ms, System.Text.Encoding.UTF8);
            using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture));

            csv.WriteField("Application");
            csv.WriteField("Générée le");
            csv.WriteField("Score global");
            csv.WriteField("Niveau de risque");
            await csv.NextRecordAsync();

            csv.WriteField(report.ApplicationName);
            csv.WriteField(report.GeneratedAt.ToString("yyyy-MM-dd HH:mm"));
            csv.WriteField($"{report.CompliancePercentage}%");
            csv.WriteField(report.RiskLevel);
            await csv.NextRecordAsync();
            await csv.NextRecordAsync();

            csv.WriteField("Chapitre");
            csv.WriteField("Nom");
            csv.WriteField("Total");
            csv.WriteField("Valides");
            csv.WriteField("Non valides");
            csv.WriteField("Non applicables");
            csv.WriteField("En attente");
            csv.WriteField("Conformité %");
            csv.WriteField("Risque");
            await csv.NextRecordAsync();

            foreach (var cat in report.CategoryBreakdown)
            {
                csv.WriteField(cat.Code);
                csv.WriteField(cat.Name);
                csv.WriteField(cat.TotalItems);
                csv.WriteField(cat.ValidCount);
                csv.WriteField(cat.NotValidCount);
                csv.WriteField(cat.NotApplicableCount);
                csv.WriteField(cat.PendingCount);
                csv.WriteField($"{cat.CompliancePercentage}%");
                csv.WriteField(cat.RiskColor switch { "success" => "Faible", "warning" => "Moyen", _ => "Élevé" });
                await csv.NextRecordAsync();
            }

            await writer.FlushAsync();
            return ms.ToArray();
        }
    }
}
