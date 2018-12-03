namespace Mega.Domain.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ITagRepository : IRepository<Tag>
    {
        Task<List<Tag>> GetTags(int limit = int.MaxValue, int offset = 0, int articleId = 0);

        Task<Tag> GetTagByOuterId(string outerKey);

        Task<int> CountTags(int articleId = 0);

        Task<List<Tag>> GetPopularTags(int countTags = 1);
    }
}