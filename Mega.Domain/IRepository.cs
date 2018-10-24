namespace Mega.Domain
{
    using System.Threading.Tasks;

    public interface IRepository<TEntity>
    {
        Task<TEntity> Create(TEntity entity);

        Task Delete(int id);

        Task<TEntity> Get(int id);

        Task Update(TEntity entity);
    }
}
