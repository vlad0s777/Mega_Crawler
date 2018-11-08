namespace Mega.Domain.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IArticleRepository : IRepository<Articles>
    {
        Task<List<Articles>> GetArticles(int limit = int.MaxValue, int offset = 0, int tagId = 0);

        Task<Articles> GetArticleInOuterId(int id);

        Task<int> CountArticles(int tagId = 0, DateTime? startDate = null, DateTime? endDate = null);
    }
}