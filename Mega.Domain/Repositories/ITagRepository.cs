namespace Mega.Domain.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ITagRepository : IRepository<Tags>
    {
        Task<List<Tags>> GetTags(int limit = int.MaxValue, int offset = 0, int articleId = 0);

        Task<Tags> GetTagInOuterId(string outerKey);

        Task<int> CountTags(int articleId = 0);

        Task<List<Tags>> GetPopularTags(int countTags = 1);
    }
}