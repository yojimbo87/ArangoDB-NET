using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Arango.Client;

namespace Arango.Tests
{
    [TestFixture()]
    public class IssueTests : IDisposable
    {
        public IssueTests()
		{
			Database.CreateTestDatabase(Database.TestDatabaseGeneral);
		}
        
        [Test()]
        public void Issue_No8_Guid_conversion()
        {
            Database.CreateTestCollection(Database.TestDocumentCollectionName, ACollectionType.Document);
            var db = new ADatabase(Database.Alias);
            
            var demo = new DemoEmployee();
            demo.SomeOtherId = Guid.NewGuid();
            demo.Name = "My name";
            
            var createResult = db.Document.Create<DemoEmployee>(Database.TestDocumentCollectionName, demo);
            
            Assert.IsTrue(createResult.Success);
            
            var getresult = db.Document.Get<DemoEmployee>(createResult.Value.ID());
            
            Assert.IsTrue(getresult.Success);
            Assert.AreEqual(demo.SomeOtherId.ToString(), getresult.Value.SomeOtherId.ToString());
        }
        
        public void Dispose()
        {
            Database.DeleteTestDatabase(Database.TestDatabaseGeneral);
        }
    }
    
    public class DemoEmployee
    {
        public Guid SomeOtherId { get; set; }
        public string Name { get; set; }
    }
}
