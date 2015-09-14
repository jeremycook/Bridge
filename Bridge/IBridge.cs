using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bridge
{
    public interface IBridge
    {
        /// <summary>
        /// Returns the matching model or null.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        TModel Get<TModel>(Guid id);

        /// <summary>
        /// Returns an <see cref="IQuery{TModel}"/> that can be filtered, sorted, paged,
        /// and converted to a list.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <returns></returns>
        IQuery<TModel> Query<TModel>();

        void Insert<TModel>(TModel model);
        void InsertRange<TModel>(IEnumerable<TModel> list);

        void Update<TModel>(Guid id, TModel model);

        void Delete<TModel>(Guid id);
        void DeleteRange<TModel>(IEnumerable<Guid> recordIds);
    }
}
