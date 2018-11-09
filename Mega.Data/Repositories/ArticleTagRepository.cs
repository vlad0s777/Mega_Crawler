namespace Mega.Data.Repositories
{
    using System.Data;
    using System.Threading.Tasks;

    using Dapper;

    using DapperExtensions;
    using DapperExtensions.Sql;

    using Mega.Domain;
    using Mega.Domain.Repositories;

    public class ArticleTagRepository : IRepository<Articles_Tags>
    {
        private readonly IDbConnection db;

        public ArticleTagRepository(IDbConnection db)
        {
            this.db = db;
            DapperAsyncExtensions.SqlDialect = new PostgreSqlDialect();
        }

        public async Task<Articles_Tags> Get(int id) => await this.db.GetAsync<Articles_Tags>(id);

        public async Task<int> Create(Articles_Tags articleTag)
        {
            var sqlQuery = "INSERT INTO articles_tags (tag_id, article_id) VALUES(@Tag_Id, @Article_Id)";
            await this.db.ExecuteAsync(sqlQuery, new { articleTag.Tag_Id, articleTag.Article_Id });
            return articleTag.Article_Id;
        }

        public async Task Update(Articles_Tags articleTag) => await this.db.UpdateAsync(articleTag);

        public async Task Delete(int id) => await this.db.DeleteAsync<Articles_Tags>(id);
    }
}
