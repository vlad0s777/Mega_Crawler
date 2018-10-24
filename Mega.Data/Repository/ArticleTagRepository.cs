namespace Mega.Data.Repository
{
    using System;
    using System.Data;
    using System.Threading.Tasks;

    using Dapper;

    using Mega.Domain;

    using Npgsql;

    public class ArticleTagRepository : IRepository<ArticleTag>
    {
        private readonly string connectionString;

        public ArticleTagRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public Task<ArticleTag> Get(int id)
        {
            throw new NotImplementedException();
        }

        public async Task Update(ArticleTag articleTag)
        {
            using (IDbConnection db = new NpgsqlConnection(this.connectionString))
            {
                var sqlQuery = "UPDATE \"ArticleTag\" SET \"TagId\" = @TagId WHERE \"ArticleId\" = @ArticleId";
                await db.ExecuteAsync(sqlQuery, new {articleTag.TagId, articleTag.ArticleId});
            }
        }

        public async Task<ArticleTag> Create(ArticleTag articleTag)
        {
            using (IDbConnection db = new NpgsqlConnection(this.connectionString))
            {
                var sqlQuery = "INSERT INTO \"ArticleTag\" (\"TagId\", \"ArticleId\") VALUES(@TagId, @ArticleId)";
                await db.ExecuteAsync(sqlQuery, new {articleTag.ArticleId, articleTag.TagId});
            }

            return articleTag;
        }

        public async Task Delete(int id)
        {
            using (IDbConnection db = new NpgsqlConnection(this.connectionString))
            {
                var sqlQuery = "DELETE FROM \"ArticleTag\" WHERE \"ArticleId\" = @id";
                await db.ExecuteAsync(sqlQuery, new {id});
            }
        }
    }
}
