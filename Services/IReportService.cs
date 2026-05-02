using WebApplicationAsp.ViewModels;

namespace WebApplicationAsp.Services
{
    public interface IReportService
    {
        Task<ReportViewModel> GenerateReportAsync(int applicationId);
        Task<DashboardViewModel> GetDashboardViewModelAsync(string userId, bool isAdmin);
    }
}
