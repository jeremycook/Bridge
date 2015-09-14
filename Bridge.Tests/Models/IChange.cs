using System;

namespace Bridge.Tests.Models
{
    public interface IChange
    {
        DateTimeOffset Created { get; }
        Guid CreatedById { get; }

        DateTimeOffset Updated { get; }
        Guid UpdatedById { get; }
    }
}