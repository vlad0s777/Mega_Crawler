namespace Mega.Data.Repository
{
    using System.Data;
    using System.Threading.Tasks;

    using Dapper;

    using Mega.Domain;

    using Npgsql;

    public class ArticleRepository : IRepository<Article>
    {
        private readonly string connectionString;

        public ArticleRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<Article> Get(int id)
        {
            using (IDbConnection db = new NpgsqlConnection(this.connectionString))
            {
                return await db.QueryFirstOrDefaultAsync<Article>("SELECT * FROM \"Articles\" WHERE \"ArticleId\" = @id", new { id });
            }
        }

        public async Task<Article> Create(Article article)
        {
                using (IDbConnection db = new NpgsqlConnection(this.connectionString))
                {
                    var sqlQuery = "INSERT INTO \"Articles\" (\"DateCreate\", \"Text\", \"Head\", \"OuterArticleId\") VALUES(@DateCreate, @Text, @Head, @OuterArticleId) RETURNING \"ArticleId\"";
                    var articleId = await db.QueryFirstOrDefaultAsync<int>(sqlQuery, new { article.DateCreate, article.Text, article.Head, article.OuterArticleId });
                    article.ArticleId = articleId;
                }

                return article; 
        }

        public async Task Update(Article article)
        {
            using (IDbConnection db = new NpgsqlConnection(this.connectionString))
            {
                var sqlQuery = "UPDATE \"Articles\" SET \"DateCreate\" = @DateCreate, \"Text\" = @Text, \"Head\" = @Head, \"OuterArticleId\" = @OuterArticleId WHERE \"ArticleId\" = @ArticleId";
                await db.ExecuteAsync(sqlQuery, new { article.DateCreate, article.Text, article.Head, article.OuterArticleId, article.ArticleId });
            }
        }

        public async Task Delete(int id)
        {
            using (IDbConnection db = new NpgsqlConnection(this.connectionString))
            {
                var sqlQuery = "DELETE FROM \"Articles\" WHERE \"ArticleId\" = @id";
                await db.ExecuteAsync(sqlQuery, new { id });
            }
        }
    }
}