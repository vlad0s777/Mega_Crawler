namespace Mega.Web.Api.Mappers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IMapper<in TSource, TDestination>
    {
        Task<TDestination> Map(TSource sourceObject);

        IEnumerable<TDestination> Map(IEnumerable<TSource> articles);
    }
}
