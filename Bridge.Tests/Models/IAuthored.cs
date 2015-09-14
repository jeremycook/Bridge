using System;

namespace Bridge.Tests.Models
{
    public  interface IAuthored
    {
        DateTimeOffset Authored { get; }
        string Author { get; }
    }
}