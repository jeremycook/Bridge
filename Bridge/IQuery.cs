using System.Collections.Generic;

namespace Bridge
{
    public interface IQuery<TModel>
    {
        IQuery<TModel> Filter(StandardFilter filter);
        IQuery<TModel> Sort(params IndexSort[] indexSorts);
        IQuery<TModel> Page(int pageSize, int currentPage = 1);
        IList<TModel> ToList();
    }
}