using WebApplicationAsp.Entities;
using WebApplicationAsp.Repository;
using WebApplicationAsp.ViewModels;

namespace WebApplicationAsp.Services
{
    public class AssessmentService : IAssessmentService
    {
        private readonly IUnitOfWork _uow;

        public AssessmentService(IUnitOfWork uow) => _uow = uow;

        public async Task<ConductViewModel> GetConductViewModelAsync(int applicationId)
        {
            var app = await _uow.Applications.GetByIdAsync(applicationId);
            var categories = await _uow.Categories.GetAllAsync(
                includeProperties: "SubCategories.Items");
            var assessments = await _uow.Assessments.GetAllAsync(
                filter: a => a.ApplicationId == applicationId);

            var assessmentDict = assessments.ToDictionary(a => a.ItemId, a => a);

            var items = new List<AssessmentItemDto>();
            int index = 0;

            foreach (var cat in categories.OrderBy(c => c.Code))
            {
                foreach (var sub in cat.SubCategories.OrderBy(s => s.Code))
                {
                    foreach (var item in sub.Items.OrderBy(i => i.Code))
                    {
                        assessmentDict.TryGetValue(item.Id, out var existing);
                        items.Add(new AssessmentItemDto
                        {
                            Index = index++,
                            ItemId = item.Id,
                            ItemCode = item.Code,
                            ItemDescription = item.Description,
                            ItemLevel = item.Level,
                            SubCategoryCode = sub.Code,
                            SubCategoryName = sub.Name,
                            CategoryCode = cat.Code,
                            CategoryName = cat.Name,
                            Status = existing?.Status ?? AssessmentStatus.Pending,
                            Comment = existing?.Comment
                        });
                    }
                }
            }

            return new ConductViewModel
            {
                ApplicationId = applicationId,
                ApplicationName = app?.Name ?? string.Empty,
                Items = items
            };
        }

        public async Task SaveAssessmentsAsync(int applicationId, List<AssessmentItemDto> items, string userId)
        {
            var existing = (await _uow.Assessments.GetAllAsync(
                filter: a => a.ApplicationId == applicationId)).ToDictionary(a => a.ItemId, a => a);

            foreach (var dto in items)
            {
                if (existing.TryGetValue(dto.ItemId, out var record))
                {
                    record.Status = dto.Status;
                    record.Comment = dto.Comment;
                    record.AssessedAt = DateTime.UtcNow;
                    record.AssessedById = userId;
                    _uow.Assessments.Update(record);
                }
                else
                {
                    await _uow.Assessments.AddAsync(new Assessment
                    {
                        ApplicationId = applicationId,
                        ItemId = dto.ItemId,
                        Status = dto.Status,
                        Comment = dto.Comment,
                        AssessedAt = DateTime.UtcNow,
                        AssessedById = userId
                    });
                }
            }

            await _uow.CompleteAsync();
        }

        public async Task<ReviewViewModel> GetReviewViewModelAsync(int applicationId)
        {
            var vm = await GetConductViewModelAsync(applicationId);
            var valid = vm.Items.Count(i => i.Status == AssessmentStatus.Valid);
            var notValid = vm.Items.Count(i => i.Status == AssessmentStatus.NotValid);
            var na = vm.Items.Count(i => i.Status == AssessmentStatus.NotApplicable);
            var pending = vm.Items.Count(i => i.Status == AssessmentStatus.Pending);
            int applicable = valid + notValid;

            return new ReviewViewModel
            {
                ApplicationId = applicationId,
                ApplicationName = vm.ApplicationName,
                Items = vm.Items,
                ValidCount = valid,
                NotValidCount = notValid,
                NotApplicableCount = na,
                PendingCount = pending,
                CompliancePercentage = applicable > 0 ? Math.Round((double)valid / applicable * 100, 1) : 0
            };
        }
    }
}
