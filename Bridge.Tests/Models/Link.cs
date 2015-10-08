using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bridge.Tests.Models
{
    public class Link : IIdentify, ITitled, ISummarize
    {
        [Obsolete("Runtime only", true)]
        public Link() { }
        public Link(string title, string summary, string url)
        {
            this.Title = title;
            this.Summary = summary;
            this.Url = url;
        }

        public Guid Id { get; set; }

        public string Title { get; protected set; }
        public string Summary { get; protected set; }
        public string Url { get; protected set; }

        public override string ToString()
        {
            return Title;
        }
    }
}
