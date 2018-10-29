namespace Mega.Data.Migrations
{
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    using Dapper;

    public class Migrator
    {
        private const string CreateMigrationQuery = "create or replace function create_constraint_if_not_exists (t_name text, c_name text, constraint_sql text) \n" +
                                                    "returns void AS $$\n" +
                                                    "begin\n" +
                                                    "    if not exists (select constraint_name from information_schema.constraint_column_usage \n" +
                                                    "                   where table_name = t_name  and constraint_name = c_name) then\n" +
                                                    "        execute constraint_sql;\n" +
                                                    "    end if;\n" +
                                                    "end;\n" +
                                                    "$$ language 'plpgsql';\n" +
                                                    "CREATE TABLE IF NOT EXISTS public.\"__MigrationsHistory\" (\"MigrationId\" character varying(150) NOT NULL); \n" +
                                                    "ALTER TABLE public.\"__MigrationsHistory\" OWNER TO postgres; \n" +
                                                    "SELECT create_constraint_if_not_exists('__MigrationsHistory', 'PK___MigrationsHistory', \n" +
                                                    "			'ALTER TABLE ONLY public.\"__MigrationsHistory\" ADD CONSTRAINT \"PK___MigrationsHistory\" PRIMARY KEY (\"MigrationId\")');";

        private readonly IDbConnection db;

        public Migrator(IDbConnection db)
        {
            this.db = db;
        }

        public async Task<string> Migrate()
        {
            await this.db.ExecuteAsync(CreateMigrationQuery);
            var migrations = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Migrations").EnumerateFiles().OrderBy(x => x.Name);
            var completedMigratins = await this.db.QueryFirstOrDefaultAsync<string>("SELECT \"MigrationId\" FROM  \"__MigrationsHistory\"");
            var returnString = string.Empty;
            foreach (var migration in migrations)
            {
                var migrationName = Path.GetFileNameWithoutExtension(migration.Name);
                if (completedMigratins != null)
                {
                    if (completedMigratins.Contains(migrationName))
                    {
                        returnString += migrationName + "already performed!<br>";
                        continue;
                    }
                }

                var query = await migration.OpenText().ReadToEndAsync();
                await this.db.ExecuteAsync(query);
                await this.db.ExecuteAsync("INSERT INTO \"__MigrationsHistory\" (\"MigrationId\") VALUES(@MigrationId)", new { MigrationId = migrationName });
                returnString += migrationName + "successfully completed<br>";
            }

            return returnString;
        }
    }
}