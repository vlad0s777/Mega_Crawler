namespace Mega.Data.Repository
{
    using System.Data;
    using System.Threading.Tasks;

    using Dapper;

    using Mega.Domain;

    using Npgsql;

    public class TagRepository : IRepository<Tag>
    {
        private readonly string connectionString;

        public TagRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<Tag> Get(int id)
        {
            using (IDbConnection db = new NpgsqlConnection(this.connectionString))
            {
                return await db.QueryFirstOrDefaultAsync<Tag>("SELECT * FROM \"Tags\" WHERE \"TagId\" = @id", new { id });
            }
        }

        public async Task<Tag> Create(Tag tag)
        {
            using (IDbConnection db = new NpgsqlConnection(this.connectionString))
            {
                var sqlQuery = "INSERT INTO \"Tags\" (\"TagKey\", \"Name\") VALUES(@TagKey, @Name) RETURNING \"TagId\"";
                var tagId = await db.QueryFirstOrDefaultAsync<int>(sqlQuery, new { tag.TagKey, tag.Name });
                tag.TagId = tagId;
            }

            return tag;
        }

        public async Task Update(Tag tag)
        {
            using (IDbConnection db = new NpgsqlConnection(this.connectionString))
            {
                var sqlQuery = "UPDATE \"Tags\" SET \"TagKey\" = @TagKey, \"Name\" = @Name WHERE \"TagId\" = @TagId";
                await db.ExecuteAsync(sqlQuery, new { tag.Name, tag.TagKey, tag.TagId});
            }
        }

        public async Task Delete(int id)
        {
            using (IDbConnection db = new NpgsqlConnection(this.connectionString))
            {
                var sqlQuery = "DELETE FROM \"Tags\" WHERE TagId = @id";
                await db.ExecuteAsync(sqlQuery, new { id });
            }
        }
    }
}
