namespace Mega.Data.Repositories
{
    using System.Data;
    using System.Threading.Tasks;

    using DapperExtensions;
    using DapperExtensions.Sql;

    using Mega.Domain;
    using Mega.Domain.Repositories;

    public class RemovedTagRepository : IRepository<Removed_Tags>
    {
        private readonly IDbConnection db;

        public RemovedTagRepository(IDbConnection db)
        {
            this.db = db;
            DapperAsyncExtensions.SqlDialect = new PostgreSqlDialect();
        }

        public async Task<Removed_Tags> Get(int id) => await this.db.GetAsync<Removed_Tags>(id);

        public async Task<int> Create(Removed_Tags removedTag) => await this.db.InsertAsync(removedTag);

        public async Task Update(Removed_Tags removedTag) => await this.db.UpdateAsync(removedTag);

        public async Task Delete(int id) => await this.db.DeleteAsync<Removed_Tags>(id);
    }
}
