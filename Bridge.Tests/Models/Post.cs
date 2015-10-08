using System;
using System.ComponentModel.DataAnnotations;

namespace Bridge.Tests.Models
{
    public class Post : IContent, IIdentify
    {
        public Post() { }
        public Post(DateTimeOffset created, Guid createdById)
        {
            this.Created = this.Updated = created;
            this.CreatedById = this.UpdatedById = createdById;
        }

        public Guid Id { get; set; }

        [Required, StringLength(100)]
        public string Title { get; set; }
        [Required, StringLength(500)]
        public string Summary { get; set; }
        [Required, StringLength(900)]
        public string Body { get; set; }

        [Required, StringLength(100)]
        public string Author { get; set; }
        [Required]
        public DateTimeOffset Authored { get; set; }

        public DateTimeOffset Created { get; set; }
        public Guid CreatedById { get; set; }
        public DateTimeOffset Updated { get; set; }
        public Guid UpdatedById { get; set; }

        public override string ToString()
        {
            return Title;
        }
    }
}
