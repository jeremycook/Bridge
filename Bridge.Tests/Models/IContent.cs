using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bridge.Tests.Models
{
    public interface IContent : ITitled, ISummarize, IAuthored, IChange
    {
        string Body { get; }
    }
}
