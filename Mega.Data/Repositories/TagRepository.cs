namespace Mega.Data.Repositories
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;

    using Dapper;

    using Mega.Domain;
    using Mega.Domain.Repositories;

    public class TagRepository : ITagRepository
    {
        private readonly IDbConnection db;

        public TagRepository(IDbConnection db)
        {
            this.db = db;
            DefaultTypeMap.MatchNamesWithUnderscores = true;
        }

        public async Task<Tag> Get(int id)
        {
            return await this.db.QueryFirstOrDefaultAsync<Tag>(@"SELECT * FROM tags WHERE tag_id = @id", new { id });
        }

        public async Task<int> Create(Tag tag)
        {
            var sqlQuery = @"INSERT INTO tags (tag_key, name) VALUES(@TagKey, @Name) RETURNING tag_id";
            return await this.db.QueryFirstOrDefaultAsync<int>(sqlQuery, new { tag.TagKey, tag.Name });
        }

        public async Task Update(Tag tag)
        {
            var sqlQuery = @"UPDATE tags SET tag_key = @TagKey, name = @Name WHERE tag_id = @TagId";
            await this.db.ExecuteAsync(sqlQuery, new { tag.Name, tag.TagKey, tag.TagId });
        }

        public async Task Delete(int id)
        {
            var sqlQuery = "DELETE FROM tags WHERE tag_id = @id";
            await this.db.ExecuteAsync(sqlQuery, new { id });
        }

        public async Task<List<Tag>> GetTags(int limit = int.MaxValue, int offset = 0, int articleId = 0)
        {
            var query = articleId == 0
                            ? @"SELECT T .* FROM tags AS T 
                                LEFT JOIN removed_tags AS TD ON TD.tag_id = T.tag_id 
                                WHERE TD.removed_tag_id IS NULL LIMIT @limit OFFSET @offset"
                            : @"SELECT T.* FROM
                                (tags AS T LEFT JOIN removed_tags AS TD ON TD.tag_id = T.tag_id) 
                                INNER JOIN articles_tags AS AT ON AT.tag_id = T.tag_id 
                                WHERE AT.article_id = @articleId AND TD.removed_tag_id IS NULL LIMIT @limit OFFSET @offset";
            return (await this.db.QueryAsync<Tag>(query, new { limit, offset, articleId })).ToList();
        }

        public async Task<Tag> GetTagByOuterId(string outerKey)
        {
            return await this.db.QueryFirstOrDefaultAsync<Tag>("SELECT * FROM tags WHERE tag_key = @outerKey", new { outerKey });
        }

        public async Task<int> CountTags(int articleId = 0)
        {
            return articleId == 0
                       ? await this.db.QueryFirstOrDefaultAsync<int>(
                             @"SELECT COUNT(*) FROM tags AS T 
                               LEFT JOIN removed_tags AS TD ON TD.tag_id = T.tag_id  
                               WHERE TD.removed_tag_id IS NULL")
                       : await this.db.QueryFirstOrDefaultAsync(
                             @"SELECT COUNT (*) FROM tags AS T 
                               INNER JOIN articles_tags AS AT ON AT.tag_id = T.tag_id 
                               LEFT JOIN removed_tags AS TD ON TD.tag_id = T.tag_id 
                               WHERE TD.removed_tag_id IS NULL AND AT.article_id = @articleId",
                             articleId);
        }

        public async Task<List<Tag>> GetPopularTags(int countTags = 1)
        {
            var query = @"WITH IDS AS (
                          SELECT AT .tag_id FROM articles_tags AS AT 
                          GROUP BY AT .tag_id ORDER BY COUNT (*) DESC) 
                          SELECT T .* FROM IDS 
                          INNER JOIN tags AS T ON IDS.tag_id = T .tag_id 
                          LEFT JOIN removed_tags AS TD ON IDS.tag_id = TD.tag_id 
                          WHERE TD.removed_tag_id IS NULL LIMIT @countTags";

            return (await this.db.QueryAsync<Tag>(query, new { countTags })).ToList();
        }
    }
}
