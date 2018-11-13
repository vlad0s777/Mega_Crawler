namespace Mega.Data.Repositories
{
    using System;
    using System.Data;
    using System.Threading.Tasks;

    using Dapper;

    using Mega.Domain;
    using Mega.Domain.Repositories;

    public class ArticleTagRepository : IRepository<ArticleTag>
    {
        private readonly IDbConnection db;

        public ArticleTagRepository(IDbConnection db)
        {
            this.db = db;
            DefaultTypeMap.MatchNamesWithUnderscores = true;
        }

        public Task<ArticleTag> Get(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<int> Create(ArticleTag articleTag)
        {
            var sqlQuery = @"INSERT INTO articles_tags (tag_id, article_id) VALUES(@Tag_Id, @Article_Id)";
            await this.db.ExecuteAsync(sqlQuery, new { Tag_Id = articleTag.TagId, Article_Id = articleTag.ArticleId });
            return articleTag.ArticleId;
        }

        public async Task Update(ArticleTag articleTag)
        {
            var sqlQuery = @"UPDATE articles_tags SET tag_id = @TagId WHERE article_id = @ArticleId";
            await this.db.ExecuteAsync(sqlQuery, new { articleTag.TagId, articleTag.ArticleId });
        }

        public async Task Delete(int id)
        {
            var sqlQuery = @"DELETE FROM articles_tags WHERE article_id = @id";
            await this.db.ExecuteAsync(sqlQuery, new { id });
        }
    }
}
