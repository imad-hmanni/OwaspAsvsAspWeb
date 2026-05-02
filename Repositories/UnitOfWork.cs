using WebApplicationAsp.Data;
using WebApplicationAsp.Entities;

namespace WebApplicationAsp.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Categories = new Repository<Category>(context);
            SubCategories = new Repository<SubCategory>(context);
            Items = new Repository<Item>(context);
            Applications = new Repository<Application>(context);
            Assessments = new Repository<Assessment>(context);
        }

        public IRepository<Category> Categories { get; }
        public IRepository<SubCategory> SubCategories { get; }
        public IRepository<Item> Items { get; }
        public IRepository<Application> Applications { get; }
        public IRepository<Assessment> Assessments { get; }

        public int Complete() => _context.SaveChanges();
        public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();
        public void Dispose() => _context.Dispose();
    }
}
