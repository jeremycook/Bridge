using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataBridge.EF.Internals;

namespace DataBridge.EF.Tests.InternalTests
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

            Assert.AreEqual(@"(exists (select * from FieldIndexes where FieldIndexes.RecordId = Records.Id and FieldIndexes.[Name] = 'Title' and FieldIndexes.[Text] = @p0)) and (exists (select * from FieldIndexes where FieldIndexes.RecordId = Records.Id and FieldIndexes.[Name] = 'Posted' and FieldIndexes.[Moment] < @p1))", where.Clause);
            Assert.AreEqual(2, where.Parameters.Count);
        }

        [TestMethod]
        public void InFilter()
        {
            var where = new SqlWhere(new And(
                new Eq(new Field("Title"), new Literal("Some Title")),
                new In(new Field("Numbers"), new Literal(1), new Literal(2), new Literal(3), new Literal(4))
            ));

            Assert.AreEqual(@"(exists (select * from FieldIndexes where FieldIndexes.RecordId = Records.Id and FieldIndexes.[Name] = 'Title' and FieldIndexes.[Text] = @p0)) and (exists (select * from FieldIndexes where FieldIndexes.RecordId = Records.Id and FieldIndexes.[Name] = 'Numbers' and FieldIndexes.[Number] in (@p1, @p2, @p3, @p4)))", where.Clause);
            Assert.AreEqual(5, where.Parameters.Count);
        }
    }
}
