using System;
using System.Collections.Generic;

namespace DataBridge
{
    public interface IBridge
    {
        /// <summary>
        /// Returns the matching model or null.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        object Get(Guid id);

        /// <summary>
        /// Returns an <see cref="IQuery{TModel}"/> that can be filtered, sorted, paged,
        /// and converted to a list.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <returns></returns>
        IQuery<TModel> Query<TModel>();

        void Insert(object model);
        void InsertRange(IEnumerable<object> models);

        void Update(Guid id, object model);

        void Delete(Guid id);
        void DeleteRange(IEnumerable<Guid> recordIds);
    }
}
