using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Paillave.Etl.EntityFrameworkCore.BulkSave;
using Paillave.Etl.EntityFrameworkCore.Core;

namespace Paillave.Etl.EntityFrameworkCoreTests
{
    [TestClass]
    public class UnitTest1
    {
        private class Class1
        {
            public int MyProperty1 { get; set; }
            public string MyProperty2 { get; set; }
            public DateTime MyProperty3 { get; set; }
        }
        private class Class2
        {
            public int MyProperty4 { get; set; }
            public string MyProperty5 { get; set; }
            public DateTime MyProperty6 { get; set; }
        }
        [TestMethod]
        public void TestMethod1()
        {
            var lst1 = Enumerable.Range(1, 100).Select(i => new Class1
            {
                MyProperty1 = i,
                MyProperty2 = i.ToString(),
                MyProperty3 = new DateTime(2000, 1, 1).AddDays(i)
            }).AsQueryable();
            var mcb = MatchCriteriaBuilder.Create(
                (Class2 i) => new { A = i.MyProperty4, B = i.MyProperty5 },
                (Class1 i) => new { A = i.MyProperty1, B = i.MyProperty2 }
                );
            var ce = mcb.GetCriteriaExpression(new Class2 { MyProperty4 = 1, MyProperty5 = "1" });
            var res = lst1.FirstOrDefault(ce);
            Assert.AreEqual(new DateTime(2000, 1, 1).AddDays(1), res.MyProperty3);
        }

        [TestMethod]
        public void TestGetSetters()
        {
            var setters = SettersExtractor.GetGetters((Class1 u) => new Class2
            {
                MyProperty4 = u.MyProperty1,
                MyProperty5 = u.MyProperty2
            });
            CollectionAssert.AreEquivalent(new[] { "MyProperty4", "MyProperty5" }, setters.Keys.ToArray());
            CollectionAssert.AreEquivalent(new[] { typeof(Class1).GetMember("MyProperty1")[0], typeof(Class1).GetMember("MyProperty2")[0] }, setters.Values.ToArray());
        }
    }
}