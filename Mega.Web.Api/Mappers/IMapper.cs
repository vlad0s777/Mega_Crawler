namespace Mega.Web.Api.Mappers
{
    using System.Collections.Generic;

    public interface IMapper<in TSource, out TDestination>
    {
        TDestination Map(TSource sourceObject);

        IEnumerable<TDestination> Map(IEnumerable<TSource> articles);
    }
}
