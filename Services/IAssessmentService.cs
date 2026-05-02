using WebApplicationAsp.ViewModels;

namespace WebApplicationAsp.Services
{
    public interface IAssessmentService
    {
        Task<ConductViewModel> GetConductViewModelAsync(int applicationId);
        Task SaveAssessmentsAsync(int applicationId, List<AssessmentItemDto> items, string userId);
        Task<ReviewViewModel> GetReviewViewModelAsync(int applicationId);
    }
}
