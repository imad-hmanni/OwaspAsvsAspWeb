using WebApplicationAsp.ViewModels;

namespace WebApplicationAsp.Services
{
    public interface IAIAssistantService
    {
        Task<AIExplanationResult> ExplainRequirementAsync(int itemId, string technology);
    }
}
