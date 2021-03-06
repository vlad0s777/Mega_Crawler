﻿namespace Mega.Data.Migrations
{
    using System;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    using Dapper;

    using Microsoft.Extensions.Logging;

    public class Migrator
    {
        private const string CreateMigrationQuery = @"create or replace function create_constraint_if_not_exists (t_name text, c_name text, constraint_sql text) 
                                                         returns void AS 
                                                         $$
                                                         begin
                                                             if not exists (select constraint_name from information_schema.constraint_column_usage 
                                                                            where table_name = t_name  and constraint_name = c_name) then
                                                                 execute constraint_sql;
                                                             end if;
                                                         end;
                                                         $$ language 'plpgsql';

                                                      create or replace function run_initial_if_migration_table_is_empty (constraint_sql text) 
                                                         returns void AS
                                                         $$
                                                         begin
                                                             if not exists (select * from __migrations_history) then
		                                                         execute constraint_sql;
	                                                         else
		                                                         TRUNCATE __migrations_history;
		                                                         PERFORM run_initial_if_migration_table_is_empty (constraint_sql);
                                                             end if;
                                                         end;
                                                         $$ language 'plpgsql';

                                                      CREATE TABLE IF NOT EXISTS __migrations_history (migration_id character varying(150) NOT NULL);
                                                      ALTER TABLE __migrations_history OWNER TO postgres; 
                                                      SELECT create_constraint_if_not_exists('__migrations_history', 'pk__migrations_history', 
                                                      'ALTER TABLE ONLY __migrations_history ADD CONSTRAINT pk__migrations_history PRIMARY KEY (migration_id)');";

        private readonly ILogger logger;

        private readonly IDbConnection db;

        public Migrator(IDbConnection db, ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory.CreateLogger<Migrator>();
            this.db = db;
        }

        public async Task<string> Migrate()
        {
            await this.db.ExecuteAsync(CreateMigrationQuery);
            var migrations = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Migrations").EnumerateFiles().OrderBy(x => x.Name);
            var completedMigrations = (await this.db.QueryAsync<string>(@"SELECT migration_id FROM __migrations_history")).ToList();
            var returnString = string.Empty;
            foreach (var migration in migrations)
            {
                var migrationName = Path.GetFileNameWithoutExtension(migration.Name);
                try
                {
                    if (completedMigrations.Contains(migrationName))
                    {
                        this.logger.LogInformation(migrationName + " already performed!");
                        continue;
                    }

                    var query = await migration.OpenText().ReadToEndAsync();

                    this.db.Open();
                    using (var transaction = this.db.BeginTransaction())
                    {
                        try
                        {
                            await this.db.ExecuteAsync(query, transaction: transaction);
                            await this.db.ExecuteAsync(
                                @"INSERT INTO __migrations_history (migration_id) VALUES(@MigrationId)",
                                new { MigrationId = migrationName },
                                transaction);
                            transaction.Commit();
                        }
                        catch (Exception e)
                        {
                            transaction.Rollback();
                            this.logger.LogError(e.Message + " in " + migrationName);
                            throw;
                        }
                    }

                    this.db.Close();

                    this.logger.LogInformation(migrationName + " successfully completed.");
                }
                catch (Exception e)
                {
                    this.logger.LogError(e.Message + " in " + migrationName);
                    throw;
                }
            }

            return returnString;
        }
    }
}