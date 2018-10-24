namespace Mega.Data
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;

    using Dapper;

    using Mega.Data.Repository;
    using Mega.Domain;

    using Npgsql;

    public class SomeReportDataProvider : ISomeReportDataProvider
    {
        private readonly string connectionString;

        private readonly IRepository<Article> articleRepository;

        private readonly IRepository<Tag> tagRepository;

        private readonly IRepository<ArticleTag> articleTagRepository;

        private readonly IRepository<TagDelete> tagDeleteRepository;

        public SomeReportDataProvider(string connectionString)
        {
            this.connectionString = connectionString;
            this.articleRepository = new ArticleRepository(connectionString);
            this.tagRepository = new TagRepository(connectionString);
            this.articleTagRepository = new ArticleTagRepository(connectionString);
            this.tagDeleteRepository = new TagDeleteRepository(connectionString);
        }

        public async Task<List<Article>> GetArticles(int limit = int.MaxValue, int offset = 0, int tagId = 0)
        {
            using (IDbConnection db = new NpgsqlConnection(this.connectionString))
            {
                var query = tagId == 0
                                ? "SELECT * FROM \"Articles\" LIMIT @limit OFFSET @offset"
                                : "SELECT A.* FROM \"Articles\" AS A INNER JOIN \"ArticleTag\" AS AT ON AT.\"ArticleId\" = A.\"ArticleId\" WHERE AT.\"TagId\" = @tagId LIMIT @limit OFFSET @offset";
                return (await db.QueryAsync<Article>(query, new { limit, offset, tagId })).ToList();
            }
        }

        public async Task<Article> GetArticle(int id, bool outer = false)
        {
            using (IDbConnection db = new NpgsqlConnection(this.connectionString))
            {
                return !outer
                           ? await this.articleRepository.Get(id)
                           : await db.QueryFirstOrDefaultAsync<Article>("SELECT * FROM \"Articles\" WHERE \"OuterArticleId\" = @id", id);
            }
        }

        public async Task<int> CountArticles(int tagId = 0, DateTime? startDate = null, DateTime? endDate = null)
        {
            var start = startDate ?? DateTime.MinValue;
            var end = endDate ?? DateTime.Now;
            using (IDbConnection db = new NpgsqlConnection(this.connectionString))
            {
                return tagId == 0
                           ? await db.QueryFirstOrDefaultAsync<int>(
                                 "SELECT COUNT(*) FROM \"Articles\" WHERE \"DateCreate\" > @start AND \"DateCreate\" < @end", new { start, end })
                           : await db.QueryFirstOrDefaultAsync<int>(
                                 "SELECT COUNT(*) FROM \"Articles\" AS A INNER JOIN \"ArticleTag\" AS AT ON AT.\"ArticleId\" = A.\"ArticleId\" WHERE AT.\"TagId\" = @tagId AND A.\"DateCreate\" > @start AND \"DateCreate\" < @end", new { start, end, tagId });
            }
        }

        public async Task<List<Tag>> GetTags(int limit = int.MaxValue, int offset = 0, int articleId = 0)
        {
            using (IDbConnection db = new NpgsqlConnection(this.connectionString))
            {
                var query = articleId == 0
                                ? $"SELECT T .* FROM \"Tags\" AS T LEFT JOIN \"TagsDelete\" AS TD ON TD.\"TagId\" = T .\"TagId\" WHERE TD.\"TagDeleteId\" IS NULL LIMIT @limit OFFSET @offset"
                                : $"SELECT T.* FROM (\"Tags\" AS T LEFT JOIN \"TagsDelete\" AS TD ON TD.\"TagId\" = T.\"TagId\") INNER JOIN \"ArticleTag\" AS AT ON AT.\"TagId\" = T.\"TagId\" WHERE AT.\"ArticleId\" = @articleId AND TD.\"TagDeleteId\" IS NULL LIMIT @limit OFFSET @offset";
                return (await db.QueryAsync<Tag>(query, new { limit, offset, articleId })).ToList();
            }
        }

        public async Task<List<TagDelete>> GetDeleteTags(int limit = int.MaxValue, int offset = 0)
        {
            using (IDbConnection db = new NpgsqlConnection(this.connectionString))
            {
                return (await db.QueryAsync<TagDelete>($"SELECT * FROM \"TagsDelete\" LIMIT @limit OFFSET @offset", new { limit, offset })).ToList();
            }
        }

        public async Task<Tag> GetTag(string outerKey)
        {
            using (IDbConnection db = new NpgsqlConnection(this.connectionString))
            {
                return await db.QueryFirstOrDefaultAsync<Tag>($"SELECT * FROM \"Tags\" WHERE \"TagKey\" = @outerKey", outerKey);
            }
        }

        public async Task<Tag> GetTag(int id) => await this.tagRepository.Get(id);

        public async Task<int> CountTags(int articleId = 0)
        {
            using (IDbConnection db = new NpgsqlConnection(this.connectionString))
            {
                return articleId == 0
                           ? await db.QueryFirstOrDefaultAsync<int>($"SELECT COUNT(*) FROM \"Tags\"")
                           : await db.QueryFirstOrDefaultAsync(
                                 $"SELECT COUNT(*) FROM \"Tags\" AS T INNER JOIN \"ArticleTag\" AS AT ON AT.\"TagId\" = T.\"TagId\" WHERE AT.\"ArticleId\" = @articleId", articleId);
            }
        }

        public async Task<List<Tag>> GetPopularTags(int countTags = 1)
        {
            var query =
                "WITH D AS (SELECT AT.\"TagId\", COUNT (*) AS tag_count FROM \"ArticleTag\" AS AT GROUP BY AT.\"TagId\" ORDER BY tag_count DESC) "
                + "SELECT T.* FROM (D INNER JOIN \"Tags\" AS T ON D.\"TagId\" = T.\"TagId\") "
                + "LEFT JOIN \"TagsDelete\" AS TD ON TD.\"TagId\" = T.\"TagId\" WHERE TD.\"TagDeleteId\" IS NULL";
            using (IDbConnection db = new NpgsqlConnection(this.connectionString))
            {
                return (await db.QueryAsync<Tag>(query)).ToList();
            }
        }

        public void Migrate()
        {
            throw new NotImplementedException();
        }

        public async Task<object> AddAsync(object entity)
        {
            switch (entity)
            {
                case Tag _:
                    return await this.tagRepository.Create(entity as Tag);
                case TagDelete _:
                    return await this.tagDeleteRepository.Create(entity as TagDelete);
                case Article _:
                    return await this.articleRepository.Create(entity as Article);
                case ArticleTag _:
                    return await this.articleTagRepository.Create(entity as ArticleTag);
                default:
                    throw new Exception("Invalid entity");
            }
        }
    }
}
