using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBridge
{
    public static class IBridgeExtensions
    {
        public static TModel Get<TModel>(this IBridge bridge, Guid id)
        {
            return (TModel)bridge.Get(id);
        }
    }
}
