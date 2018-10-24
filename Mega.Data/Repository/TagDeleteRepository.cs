namespace Mega.Data.Repository
{
    using System.Data;
    using System.Threading.Tasks;

    using Dapper;

    using Mega.Domain;

    using Npgsql;

    public class TagDeleteRepository : IRepository<TagDelete>
    {
        private readonly string connectionString;

        public TagDeleteRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<TagDelete> Get(int id)
        {
            using (IDbConnection db = new NpgsqlConnection(this.connectionString))
            {
                return await db.QueryFirstOrDefaultAsync<TagDelete>($"SELECT * FROM \"TagsDelete\" WHERE \"TagDeleteId\" = @id", new { id });
            }
        }

        public async Task Update(TagDelete tagDelete)
        {
            using (IDbConnection db = new NpgsqlConnection(this.connectionString))
            {
                var sqlQuery = "UPDATE \"TagDelete\" SET \"TagId\"=@TagId, \"DateDelete\"=@DateDelete WHERE \"TagDeleteId\" = @TagDeleteId";
                await db.ExecuteAsync(sqlQuery, new { tagDelete.TagId, tagDelete.DateDelete, tagDelete.TagDeleteId });
            }
        }

        public async Task<TagDelete> Create(TagDelete tagDelete)
        {
            using (IDbConnection db = new NpgsqlConnection(this.connectionString))
            {
                var sqlQuery = "INSERT INTO \"TagsDelete\" (\"TagId\", \"DateDelete\") VALUES(@TagId, @DateDelete) RETURNING \"TagDeleteId\"";
                var tagDeleteId = await db.QueryFirstOrDefaultAsync<int>(sqlQuery, new { tagDelete.TagId, tagDelete.DateDelete });
                tagDelete.TagId = tagDeleteId;
            }

            return tagDelete;
        }

        public async Task Delete(int id)
        {
            using (IDbConnection db = new NpgsqlConnection(this.connectionString))
            {
                await db.ExecuteAsync("DELETE FROM \"TagsDelete\" WHERE \"TagDeleteId\" = @id", new { id });
            }
        }
    }
}
