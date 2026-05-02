using WebApplicationAsp.Entities;
using WebApplicationAsp.Repository;
using WebApplicationAsp.ViewModels;

namespace WebApplicationAsp.Services
{
    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _uow;

        public ReportService(IUnitOfWork uow) => _uow = uow;

        public async Task<ReportViewModel> GenerateReportAsync(int applicationId)
        {
            var app = await _uow.Applications.GetByIdAsync(applicationId);
            var categories = await _uow.Categories.GetAllAsync(includeProperties: "SubCategories.Items");
            var assessments = (await _uow.Assessments.GetAllAsync(
                filter: a => a.ApplicationId == applicationId)).ToList();

            var allItems = categories
                .SelectMany(c => c.SubCategories)
                .SelectMany(s => s.Items)
                .ToList();

            var valid = assessments.Count(a => a.Status == AssessmentStatus.Valid);
            var notValid = assessments.Count(a => a.Status == AssessmentStatus.NotValid);
            var na = assessments.Count(a => a.Status == AssessmentStatus.NotApplicable);
            var pending = assessments.Count(a => a.Status == AssessmentStatus.Pending);
            int applicable = valid + notValid;
            double compliance = applicable > 0 ? Math.Round((double)valid / applicable * 100, 1) : 0;

            var (riskLevel, riskColor) = GetRisk(compliance, pending, allItems.Count);

            var breakdown = categories.Select(cat =>
            {
                var catItemIds = cat.SubCategories.SelectMany(s => s.Items).Select(i => i.Id).ToHashSet();
                var catAss = assessments.Where(a => catItemIds.Contains(a.ItemId)).ToList();
                var cv = catAss.Count(a => a.Status == AssessmentStatus.Valid);
                var cnv = catAss.Count(a => a.Status == AssessmentStatus.NotValid);
                int ca = cv + cnv;
                double cc = ca > 0 ? Math.Round((double)cv / ca * 100, 1) : 0;
                return new CategoryReportViewModel
                {
                    Code = cat.Code,
                    Name = cat.Name,
                    TotalItems = catItemIds.Count,
                    ValidCount = cv,
                    NotValidCount = cnv,
                    NotApplicableCount = catAss.Count(a => a.Status == AssessmentStatus.NotApplicable),
                    PendingCount = catAss.Count(a => a.Status == AssessmentStatus.Pending),
                    CompliancePercentage = cc,
                    RiskColor = GetRiskColor(cc)
                };
            }).OrderBy(c => c.CompliancePercentage).ToList();

            var recommendations = BuildRecommendations(breakdown, compliance);

            return new ReportViewModel
            {
                ApplicationId = applicationId,
                ApplicationName = app?.Name ?? "N/A",
                ApplicationDescription = app?.Description,
                GeneratedAt = DateTime.UtcNow,
                TotalRequirements = allItems.Count,
                ValidCount = valid,
                NotValidCount = notValid,
                NotApplicableCount = na,
                PendingCount = pending,
                CompliancePercentage = compliance,
                RiskLevel = riskLevel,
                RiskColor = riskColor,
                CategoryBreakdown = breakdown,
                Recommendations = recommendations
            };
        }

        public async Task<DashboardViewModel> GetDashboardViewModelAsync(string userId, bool isAdmin)
        {
            var applications = isAdmin
                ? await _uow.Applications.GetAllAsync()
                : await _uow.Applications.GetAllAsync(filter: a => a.OwnerId == userId);

            var appList = applications.ToList();
            var appIds = appList.Select(a => a.Id).ToHashSet();

            var assessments = (await _uow.Assessments.GetAllAsync(
                filter: a => appIds.Contains(a.ApplicationId))).ToList();

            var categories = await _uow.Categories.GetAllAsync(includeProperties: "SubCategories.Items");
            var allItems = categories.SelectMany(c => c.SubCategories).SelectMany(s => s.Items).ToList();

            int valid = assessments.Count(a => a.Status == AssessmentStatus.Valid);
            int notValid = assessments.Count(a => a.Status == AssessmentStatus.NotValid);
            int na = assessments.Count(a => a.Status == AssessmentStatus.NotApplicable);
            int pending = assessments.Count(a => a.Status == AssessmentStatus.Pending);
            int applicable = valid + notValid;
            double compliance = applicable > 0 ? Math.Round((double)valid / applicable * 100, 1) : 0;

            var catCompliance = categories.Select(cat =>
            {
                var catItemIds = cat.SubCategories.SelectMany(s => s.Items).Select(i => i.Id).ToHashSet();
                var catAss = assessments.Where(a => catItemIds.Contains(a.ItemId)).ToList();
                int cv = catAss.Count(a => a.Status == AssessmentStatus.Valid);
                int cnv = catAss.Count(a => a.Status == AssessmentStatus.NotValid);
                int ca = cv + cnv;
                return new CategoryComplianceViewModel
                {
                    Code = cat.Code,
                    Name = cat.Name,
                    TotalItems = catItemIds.Count,
                    ValidCount = cv,
                    NotValidCount = cnv,
                    CompliancePercentage = ca > 0 ? Math.Round((double)cv / ca * 100, 1) : 0
                };
            }).ToList();

            var recentApps = appList.OrderByDescending(a => a.CreatedAt).Take(5).Select(a =>
            {
                var appAss = assessments.Where(x => x.ApplicationId == a.Id).ToList();
                int av = appAss.Count(x => x.Status == AssessmentStatus.Valid);
                int anv = appAss.Count(x => x.Status == AssessmentStatus.NotValid);
                int aa = av + anv;
                double ac = aa > 0 ? Math.Round((double)av / aa * 100, 1) : 0;
                return new ApplicationSummaryViewModel
                {
                    Id = a.Id,
                    Name = a.Name,
                    CompliancePercentage = ac,
                    RiskLevel = GetRiskLevel(ac),
                    CreatedAt = a.CreatedAt
                };
            }).ToList();

            var (riskLevel, _) = GetRisk(compliance, pending, allItems.Count);

            return new DashboardViewModel
            {
                TotalApplications = appList.Count,
                TotalRequirements = allItems.Count,
                TotalAssessments = assessments.Count,
                OverallCompliance = compliance,
                RiskLevel = riskLevel,
                ValidCount = valid,
                NotValidCount = notValid,
                NotApplicableCount = na,
                PendingCount = pending,
                CategoryCompliance = catCompliance,
                RecentApplications = recentApps
            };
        }

        private static (string level, string color) GetRisk(double compliance, int pending, int total)
        {
            if (pending == total) return ("Non évalué", "secondary");
            return compliance switch
            {
                >= 80 => ("Faible", "success"),
                >= 50 => ("Moyen", "warning"),
                _ => ("Élevé", "danger")
            };
        }

        private static string GetRiskLevel(double compliance) => compliance switch
        {
            >= 80 => "Faible",
            >= 50 => "Moyen",
            _ => "Élevé"
        };

        private static string GetRiskColor(double compliance) => compliance switch
        {
            >= 80 => "success",
            >= 50 => "warning",
            _ => "danger"
        };

        private static List<string> BuildRecommendations(List<CategoryReportViewModel> breakdown, double overall)
        {
            var recs = new List<string>();
            if (overall < 50)
                recs.Add("Score global critique : prioriser les chapitres les plus faibles avant toute mise en production.");
            var weakest = breakdown.Where(c => c.CompliancePercentage < 50 && c.ValidCount + c.NotValidCount > 0).Take(3);
            foreach (var w in weakest)
                recs.Add($"Chapitre {w.Code} ({w.Name}) : {w.CompliancePercentage}% de conformité — action immédiate requise.");
            if (breakdown.Any(c => c.PendingCount > 0))
                recs.Add("Des exigences sont encore en statut 'Pending' — finaliser l'évaluation pour un rapport complet.");
            if (recs.Count == 0)
                recs.Add("Bonne conformité globale. Continuer la surveillance et planifier les re-évaluations périodiques.");
            return recs;
        }
    }
}
