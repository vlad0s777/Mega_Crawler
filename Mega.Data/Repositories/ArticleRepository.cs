namespace Mega.Data.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;

    using Dapper;

    using Mega.Domain;
    using Mega.Domain.Repositories;

    public class ArticleRepository : IArticleRepository
    {
        private readonly IDbConnection db;

        public ArticleRepository(IDbConnection db)
        {
            this.db = db;
            DefaultTypeMap.MatchNamesWithUnderscores = true;
        }

        public async Task<Article> Get(int id)
        {
            return await this.db.QueryFirstOrDefaultAsync<Article>(@"SELECT * FROM articles WHERE article_id = @id", new { id });
        }

        public async Task<int> Create(Article article)
        {
            var sqlQuery = @"INSERT INTO articles (date_create, text, head, outer_article_id) VALUES(@DateCreate, @Text, @Head, @OuterArticleId) RETURNING article_id";
            return await this.db.QueryFirstOrDefaultAsync<int>(sqlQuery, new { article.DateCreate, article.Text, article.Head, article.OuterArticleId });
        }

        public async Task Update(Article article)
        {
            var sqlQuery = @"UPDATE articles SET date_create = @DateCreate, text = @Text, head = @Head, outer_article_id = @OuterArticleId WHERE article_id = @ArticleId";
            await this.db.ExecuteAsync(sqlQuery, new { article.DateCreate, article.Text, article.Head, article.OuterArticleId, article.ArticleId });
        }

        public async Task Delete(int id)
        {
            var sqlQuery = @"DELETE FROM article WHERE article_id = @id";
            await this.db.ExecuteAsync(sqlQuery, new { id });
        }

        public async Task<List<Article>> GetArticles(int limit = int.MaxValue, int offset = 0, int tagId = 0)
        {
            var query = tagId == 0
                            ? @"SELECT * FROM articles  
                                LIMIT @limit OFFSET @offset"
                            : @"SELECT A.* FROM articles AS A 
                                INNER JOIN articles_tags AS AT ON AT.article_id = A.article_id 
                                WHERE AT.tag_id = @tagId LIMIT @limit OFFSET @offset";
            return (await this.db.QueryAsync<Article>(query, new { limit, offset, tagId })).ToList();
        }

        public async Task<Article> GetArticleByOuterId(int id)
        {
            var query = @"SELECT * FROM articles WHERE outer_article_id = @id";
            return await this.db.QueryFirstOrDefaultAsync<Article>(query, new { id });
        }

        public async Task<int> CountArticles(int tagId = 0, DateTime? startDate = null, DateTime? endDate = null)
        {
            var start = startDate ?? DateTime.MinValue;
            var end = endDate ?? DateTime.Now;

            return tagId == 0
                       ? await this.db.QueryFirstOrDefaultAsync<int>(
                             @"SELECT COUNT(*) FROM articles WHERE date_create > @start AND date_create < @end",
                             new { start, end })
                       : await this.db.QueryFirstOrDefaultAsync<int>(
                             @"SELECT COUNT(*) FROM articles AS A 
                               INNER JOIN articles_tags AS AT ON AT.article_id = A.article_id
                               WHERE AT.tag_id = @tagId AND A.date_create > @start AND date_create < @end", 
                             new { start, end, tagId });
        }
    }
}