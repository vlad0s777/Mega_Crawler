namespace Mega.Data.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;

    using Dapper;

    using DapperExtensions;
    using DapperExtensions.Sql;

    using Mega.Domain;
    using Mega.Domain.Repositories;

    public class ArticleRepository : IArticleRepository
    {
        private readonly IDbConnection db;

        public ArticleRepository(IDbConnection db)
        {
            this.db = db;
            DapperAsyncExtensions.SqlDialect = new PostgreSqlDialect();
        }

        public async Task<Articles> Get(int id)
        {
            return await this.db.GetAsync<Articles>(id);

            // return await this.db.QueryFirstOrDefaultAsync<Articles>("SELECT * FROM articles WHERE article_id = @id", new { id });
        }

        public async Task<int> Create(Articles article)
        {
            return await this.db.InsertAsync(article);

            // var sqlQuery = "INSERT INTO articles (date_create, text, head, outer_article_id) VALUES(@DateCreate, @Text, @Head, @OuterArticleId) RETURNING article_id";
            // return await this.db.QueryFirstOrDefaultAsync<int>(sqlQuery, new { article.Date_Create, article.Text, article.Head, article.Outer_Article_Id });
        }

        public async Task Update(Articles article) => await this.db.UpdateAsync(article);

        public async Task Delete(int id) => await this.db.DeleteAsync<Articles>(id);

        public async Task<List<Articles>> GetArticles(int limit = int.MaxValue, int offset = 0, int tagId = 0)
        {
            var query = tagId == 0
                            ? "SELECT * FROM Articles "  
                              + "LIMIT @limit OFFSET @offset"
                            : "SELECT A.* FROM Articles AS A "
                              + "INNER JOIN articles_tags AS AT ON AT.article_id = A.article_id "
                              + "WHERE AT.tag_id = @tagId LIMIT @limit OFFSET @offset";
            return (await this.db.QueryAsync<Articles>(query, new { limit, offset, tagId })).ToList();
        }

        public async Task<Articles> GetArticleInOuterId(int id)
        {
            var query = "SELECT * FROM articles WHERE outer_article_id = @id";
            return await this.db.QueryFirstOrDefaultAsync<Articles>(query, new { id });
        }

        public async Task<int> CountArticles(int tagId = 0, DateTime? startDate = null, DateTime? endDate = null)
        {
            var start = startDate ?? DateTime.MinValue;
            var end = endDate ?? DateTime.Now;

            return tagId == 0
                       ? await this.db.QueryFirstOrDefaultAsync<int>(
                             "SELECT COUNT(*) FROM articles WHERE date_create > @start AND date_create < @end",
                             new { start, end })
                       : await this.db.QueryFirstOrDefaultAsync<int>(
                             "SELECT COUNT(*) FROM articles AS A "
                             + "INNER JOIN articles_tags AS AT ON AT.article_id = A.article_id "
                             + "WHERE AT.tag_id = @tagId AND A.date_create > @start AND date_create < @end", 
                             new { start, end, tagId });
        }
    }
}