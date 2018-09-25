namespace Mega.Data
{
    using System;
    using System.IO;

    using Microsoft.EntityFrameworkCore.Design;
    using Microsoft.Extensions.Configuration;

    public class DataContextFactory : IDesignTimeDbContextFactory<DataContext>
    {
        private static string connectionString;

        public DataContext CreateDbContext() => CreateDbContext(Array.Empty<string>());

        public DataContext CreateDbContext(string[] args)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                LoadConnectionString();
            }

            return new DataContext(connectionString);
        }

        private static void LoadConnectionString()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory() + "\\Properties")
                .AddJsonFile("Mega.Crawler.appsettings.json", false, true)
                .AddJsonFile($"Mega.Crawler.appsettings.development.json", true); 

            var config = builder.Build();
            connectionString = config.GetConnectionString("DefaultConnection");
        }
    }
}
