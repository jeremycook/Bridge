using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Bridge
{
    public interface IQuery<TModel>
    {
        IQuery<TModel> Filter(Expression<Func<IIndex, bool>> filter);
        IQuery<TModel> Sort(params IndexSort[] indexSorts);
        IQuery<TModel> Page(int pageSize, int currentPage = 1);
        IList<TModel> ToList();
    }
}