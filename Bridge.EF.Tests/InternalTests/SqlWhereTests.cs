using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bridge.EF.Internals;

namespace Bridge.EF.Tests.InternalTests
{
    [TestClass]
    public class SqlWhereTests
    {
        [TestMethod]
        public void AndFilter()
        {
            var where = new SqlWhere(new And(
                new Eq(new Field("Title"), new Literal("Some Title")),
                new Lt(new Field("Posted"), new Literal(DateTimeOffset.Now))
            ));

            Assert.AreEqual(@"(Indices.[Name] = 'Title' and Indices.[Value] = @p0) and (Indices.[Name] = 'Posted' and Indices.[Value] < @p1)", where.Clause);
            Assert.AreEqual(2, where.Parameters.Count);
        }

        [TestMethod]
        public void InFilter()
        {
            var where = new SqlWhere(new And(
                new Eq(new Field("Title"), new Literal("Some Title")),
                new In(new Field("Numbers"), new Literal(1), new Literal(2), new Literal(3), new Literal(4))
            ));

            Assert.AreEqual(@"(Indices.[Name] = 'Title' and Indices.[Value] = @p0) and (Indices.[Name] = 'Numbers' and Indices.[Value] in (@p1, @p2, @p3, @p4))", where.Clause);
            Assert.AreEqual(5, where.Parameters.Count);
        }
    }
}
