using System;
using NUnit.Framework;
using Arango.Client;

namespace Arango.Tests.ExceptionTests
{
    [TestFixture()]
    public class ExceptionTests : IDisposable
    {
        [Test()]
        public void Should_throw_exception()
        {
            // given
            ASettings.ThrowExceptions = true;

            // when
            var arangoException = Assert.Throws<AException>(() => {
                var db = new ADatabase(Database.SystemAlias);
                var resultCreate = db.Create("*/-+");
            });

            // then
            Assert.IsNotNull(arangoException);
            Assert.AreEqual(400, arangoException.StatusCode);
            Assert.IsNotNullOrEmpty(arangoException.Message);
        }

        public void Dispose()
        {
            ASettings.ThrowExceptions = false;
        }
    }
}
