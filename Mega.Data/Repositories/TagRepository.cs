namespace Mega.Data.Repositories
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;

    using Dapper;

    using DapperExtensions;
    using DapperExtensions.Sql;

    using Mega.Domain;
    using Mega.Domain.Repositories;

    public class TagRepository : ITagRepository
    {
        private readonly IDbConnection db;

        public TagRepository(IDbConnection db)
        {
            this.db = db;
            DapperAsyncExtensions.SqlDialect = new PostgreSqlDialect();
        }

        public async Task<Tags> Get(int id) => await this.db.GetAsync<Tags>(id);

        public async Task<int> Create(Tags tag) => await this.db.InsertAsync(tag);

        public async Task Update(Tags tag) => await this.db.UpdateAsync(tag);

        public async Task Delete(int id) => await this.db.DeleteAsync<Tags>(id);

        public async Task<List<Tags>> GetTags(int limit = int.MaxValue, int offset = 0, int articleId = 0)
        {
            var query = articleId == 0
                            ? "SELECT T .* FROM tags AS T "
                              + "LEFT JOIN removed_tags AS TD ON TD.tag_id = T.tag_id "
                              + "WHERE TD.removed_tag_id IS NULL LIMIT @limit OFFSET @offset"
                            : "SELECT T.* FROM "
                              + "(tags AS T LEFT JOIN removed_tags AS TD ON TD.tag_id = T.tag_id) "
                              + "INNER JOIN articles_tags AS AT ON AT.tag_id = T.tag_id "
                              + "WHERE AT.article_id = @articleId AND TD.removed_tag_id IS NULL LIMIT @limit OFFSET @offset";
            return (await this.db.QueryAsync<Tags>(query, new { limit, offset, articleId })).ToList();
        }

        public async Task<Tags> GetTagInOuterId(string outerKey)
        {
            return await this.db.QueryFirstOrDefaultAsync<Tags>(@"SELECT * FROM tags WHERE tag_key = @outerKey", new { outerKey });
        }

        public async Task<int> CountTags(int articleId = 0)
        {
            return articleId == 0
                       ? await this.db.QueryFirstOrDefaultAsync<int>(
                             "SELECT COUNT(*) FROM tags AS T "
                             + "LEFT JOIN removed_tags AS TD ON TD.tag_id = T.tag_id " 
                             + "WHERE TD.removed_tag_id IS NULL")
                       : await this.db.QueryFirstOrDefaultAsync(
                             "SELECT COUNT (*) FROM tags AS T "
                             + "INNER JOIN articles_tags AS AT ON AT.tag_id = T.tag_id " 
                             + "LEFT JOIN removed_tags AS TD ON TD.tag_id = T.tag_id "
                             + "WHERE TD.removed_tag_id IS NULL AND AT.article_id = @articleId",
                             articleId);
        }

        public async Task<List<Tags>> GetPopularTags(int countTags = 1)
        {
            var query = "WITH IDS AS ("
                        + "SELECT AT .tag_id FROM articles_tags AS AT "
                        + "GROUP BY AT .tag_id ORDER BY COUNT (*) DESC) " 
                        + "SELECT T .* FROM IDS " 
                        + "INNER JOIN tags AS T ON IDS.tag_id = T .tag_id " 
                        + "LEFT JOIN removed_tags AS TD ON IDS.tag_id = TD.tag_id "
                        + "WHERE TD.removed_tag_id IS NULL LIMIT @countTags";

            return (await this.db.QueryAsync<Tags>(query, new { countTags })).ToList();
        }
    }
}
