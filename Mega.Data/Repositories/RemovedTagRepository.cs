namespace Mega.Data.Repositories
{
    using System.Data;
    using System.Threading.Tasks;

    using Dapper;

    using Mega.Domain;
    using Mega.Domain.Repositories;

    public class RemovedTagRepository : IRepository<RemovedTag>
    {
        private readonly IDbConnection db;

        public RemovedTagRepository(IDbConnection db)
        {
            this.db = db;
            DefaultTypeMap.MatchNamesWithUnderscores = true;
        }

        public async Task<RemovedTag> Get(int id)
        {
            return await this.db.QueryFirstOrDefaultAsync<RemovedTag>(@"SELECT * FROM removed_tags WHERE removed_tag_id = @id", new { id });
        }

        public async Task<int> Create(RemovedTag removedTag)
        {
            var sqlQuery = @"INSERT INTO removed_tags (tag_id, deletion_date) VALUES(@TagId, @DeletionDate) RETURNING removed_tag_id";
            return await this.db.QueryFirstOrDefaultAsync<int>(sqlQuery, new { removedTag.TagId, removedTag.DeletionDate });
        }

        public async Task Update(RemovedTag removedTag)
        {
            var sqlQuery = @"UPDATE removed_tags SET tag_id=@TagId, deletion_date=@DeletionDate WHERE removed_tag_id = @RemovedTagId";
            await this.db.ExecuteAsync(sqlQuery, new { removedTag.TagId, removedTag.DeletionDate, removedTag.RemovedTagId });
        }

        public async Task Delete(int id)
        {
            await this.db.ExecuteAsync(@"DELETE FROM removed_tags WHERE removed_tag_id = @id", new { id });
        }
    }
}
