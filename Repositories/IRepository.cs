using System.Linq.Expressions;

namespace WebApplicationAsp.Repository
{
    public interface IRepository<T> where T : class
    {
        // Sync (ce que tu as déjà)
        IEnumerable<T> GetAll();
        T GetById(int id);
        void Add(T entity);
        void Update(T entity);
        void Remove(T entity);

        // Async + Include (NOUVEAU)
        Task<IEnumerable<T>> GetAllAsync(
            Expression<Func<T, bool>> filter = null,
            string includeProperties = null
        );

        Task<T> GetByIdAsync(int id);
        Task AddAsync(T entity);
    }
}