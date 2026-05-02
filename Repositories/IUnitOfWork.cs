using WebApplicationAsp.Entities;

namespace WebApplicationAsp.Repository
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Category> Categories { get; }
        IRepository<SubCategory> SubCategories { get; }
        IRepository<Item> Items { get; }
        IRepository<Application> Applications { get; }
        IRepository<Assessment> Assessments { get; }

        int Complete();
        Task<int> CompleteAsync();
    }
}
