namespace WebApplicationAsp.Services
{
    public interface IExportService
    {
        Task<byte[]> ExportToCsvAsync(int applicationId);
    }
}
