using System;

namespace DBridge.Tests.Models
{
    public  interface IAuthored
    {
        DateTimeOffset Authored { get; }
        string Author { get; }
    }
}