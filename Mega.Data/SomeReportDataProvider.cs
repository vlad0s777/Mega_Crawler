namespace Mega.Data
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;

    using Dapper;

    using Mega.Data.Migrations;
    using Mega.Domain;

    public class SomeReportDataProvider : ISomeReportDataProvider
    {
        private readonly IDbConnection db;

        private readonly Migrator migrator;

        public SomeReportDataProvider(IDbConnection db, Migrator migrator)
        {
            this.db = db;
            this.migrator = migrator;
        }

        public async Task<List<Article>> GetArticles(int limit = int.MaxValue, int offset = 0, int tagId = 0)
        {
            var query = tagId == 0
                            ? "SELECT * FROM \"Articles\" LIMIT @limit OFFSET @offset"
                            : "SELECT A.* FROM \"Articles\" AS A INNER JOIN \"ArticleTag\" AS AT ON AT.\"ArticleId\" = A.\"ArticleId\" WHERE AT.\"TagId\" = @tagId LIMIT @limit OFFSET @offset";
            return (await this.db.QueryAsync<Article>(query, new { limit, offset, tagId })).ToList();
        }

        public async Task<Article> GetArticle(int id, bool outer = false)
        {
            var colId = !outer ? "\"ArticleId\"" : "\"OuterArticleId\"";
            return await this.db.QueryFirstOrDefaultAsync<Article>("SELECT * FROM \"Articles\" WHERE @colId = @id", new { colId, id });
        }

        public async Task<int> CountArticles(int tagId = 0, DateTime? startDate = null, DateTime? endDate = null)
        {
            var start = startDate ?? DateTime.MinValue;
            var end = endDate ?? DateTime.Now;

            return tagId == 0
                        ? await this.db.QueryFirstOrDefaultAsync<int>(
                                "SELECT COUNT(*) FROM \"Articles\" WHERE \"DateCreate\" > @start AND \"DateCreate\" < @end", new { start, end })
                        : await this.db.QueryFirstOrDefaultAsync<int>(
                                "SELECT COUNT(*) FROM \"Articles\" AS A INNER JOIN \"ArticleTag\" AS AT ON AT.\"ArticleId\" = A.\"ArticleId\" WHERE AT.\"TagId\" = @tagId AND A.\"DateCreate\" > @start AND \"DateCreate\" < @end", new { start, end, tagId });
        }

        public async Task<List<Tag>> GetTags(int limit = int.MaxValue, int offset = 0, int articleId = 0)
        {
            var query = articleId == 0
                            ? $"SELECT T .* FROM \"Tags\" AS T LEFT JOIN \"RemovedTags\" AS TD ON TD.\"TagId\" = T .\"TagId\" WHERE TD.\"RemovedTagId\" IS NULL LIMIT @limit OFFSET @offset"
                            : $"SELECT T.* FROM (\"Tags\" AS T LEFT JOIN \"RemovedTags\" AS TD ON TD.\"TagId\" = T.\"TagId\") INNER JOIN \"ArticleTag\" AS AT ON AT.\"TagId\" = T.\"TagId\" WHERE AT.\"ArticleId\" = @articleId AND TD.\"RemovedTagId\" IS NULL LIMIT @limit OFFSET @offset";
            return (await this.db.QueryAsync<Tag>(query, new { limit, offset, articleId })).ToList();
        }

        public async Task<List<RemovedTag>> GetRemovedTags(int limit = int.MaxValue, int offset = 0)
        {
            return (await this.db.QueryAsync<RemovedTag>($"SELECT * FROM \"RemovedTags\" LIMIT @limit OFFSET @offset", new { limit, offset }))
                .ToList();
        }

        public async Task<Tag> GetTag(string outerKey) => await this.db.QueryFirstOrDefaultAsync<Tag>($"SELECT * FROM \"Tags\" WHERE \"TagKey\" = @outerKey", outerKey);

        public async Task<Tag> GetTag(int id)
        {
            return await this.db.QueryFirstOrDefaultAsync<Tag>("SELECT * FROM \"Tags\" WHERE \"TagId\" = @id", new { id });
        }

        public async Task<int> CountTags(int articleId = 0)
        {
            return articleId == 0
                        ? await this.db.QueryFirstOrDefaultAsync<int>("SELECT COUNT(*) FROM \"Tags\" AS T LEFT JOIN \"RemovedTags\" AS TD ON TD.\"TagId\" = T.\"TagId\" WHERE TD.\"RemovedTagId\" IS NULL")
                        : await this.db.QueryFirstOrDefaultAsync(
                                "SELECT\r\n\tCOUNT (*)\r\nFROM \"Tags\" AS T\r\nINNER JOIN \"ArticleTag\" AS AT ON AT.\"TagId\" = T.\"TagId\" \r\nLEFT JOIN \"RemovedTags\" AS TD ON TD.\"TagId\" = T.\"TagId\" \r\nWHERE TD.\"RemovedTagId\" IS NULL AND AT.\"ArticleId\" = @articleId",
                                articleId);
        }

        public async Task<List<Tag>> GetPopularTags(int countTags = 1)
        {
            var query = "WITH IDS AS (" +
                        "	SELECT AT .\"TagId\"" +
                        "	FROM \"ArticleTag\" AS AT" +
                        "	GROUP BY AT .\"TagId\"" +
                        "	ORDER BY COUNT (*) DESC) " +
                        "SELECT T .* FROM IDS " +
                        "INNER JOIN \"Tags\" AS T ON IDS.\"TagId\" = T .\"TagId\" " +
                        "LEFT JOIN \"RemovedTags\" AS TD ON IDS.\"TagId\" = TD.\"TagId\" " +
                        "WHERE TD.\"RemovedTagId\" IS NULL " +
                        "LIMIT @countTags";

            return (await this.db.QueryAsync<Tag>(query, new { countTags })).ToList();
        }

        public async Task<string> Migrate()
        {
            return await this.migrator.Migrate();
        }

        public async Task<object> AddAsync(object entity)
        {
            switch (entity)
            {
                case Tag _:
                    var tag = (Tag)entity;
                    var tagId = await this.db.QueryFirstOrDefaultAsync<int>("INSERT INTO \"Tags\" (\"TagKey\", \"Name\") VALUES(@TagKey, @Name) RETURNING \"TagId\"", new { tag.TagKey, tag.Name });
                    tag.TagId = tagId;
                    return tag;
                case RemovedTag _:
                    var removedTag = (RemovedTag)entity;
                    var removedTagId = await this.db.QueryFirstOrDefaultAsync<int>("INSERT INTO \"RemovedTags\" (\"TagId\", \"DeletionDate\") VALUES(@TagId, @DeletionDate) RETURNING \"RemovedTagId\"", new { removedTag.TagId, removedTag.DeletionDate });
                    removedTag.TagId = removedTagId;
                    return removedTag;
                case Article _:
                    var article = (Article)entity;
                    var articleId = await this.db.QueryFirstOrDefaultAsync<int>("INSERT INTO \"Articles\" (\"DateCreate\", \"Text\", \"Head\", \"OuterArticleId\") VALUES(@DateCreate, @Text, @Head, @OuterArticleId) RETURNING \"ArticleId\"", new { article.DateCreate, article.Text, article.Head, article.OuterArticleId });
                    article.ArticleId = articleId;
                    return article;
                case ArticleTag _:
                    var articleTag = (ArticleTag)entity;
                    await this.db.ExecuteAsync("INSERT INTO \"ArticleTag\" (\"TagId\", \"ArticleId\") VALUES(@TagId, @ArticleId)", new { articleTag.ArticleId, articleTag.TagId });
                    return articleTag;
                default:
                    throw new Exception("Invalid entity");
            }
        }
    }
}
