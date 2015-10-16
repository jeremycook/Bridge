using System;

namespace DataBridge.Tests.Models
{
    public  interface IAuthored
    {
        DateTimeOffset Authored { get; }
        string Author { get; }
    }
}