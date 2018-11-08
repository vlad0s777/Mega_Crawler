namespace Mega.Domain.Repositories
{
    using System.Threading.Tasks;

    public interface IRepository<TEntity>
    {
        Task<int> Create(TEntity entity);

        Task Delete(int id);

        Task<TEntity> Get(int id);

        Task Update(TEntity entity);
    }
}
