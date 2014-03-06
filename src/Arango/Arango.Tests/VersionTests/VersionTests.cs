using System;
using NUnit.Framework;
using Arango.Client;
using System.Text.RegularExpressions;

namespace Arango.Tests.VersionTests
{
    [TestFixture()]
    public class ArangoVersionTests : IDisposable
    {
    	public ArangoVersionTests()
    	{
    	       Database.CreateTestDatabase(Database.TestDatabaseGeneral);
    	}
    	
        [Test()]
        public void Should_read_version()
        {
        	var db = Database.GetTestDatabase();
        	ArangoVersion version = db.Version.Get();
        	
        	Assert.IsTrue(version.Major >= 1);
        	Assert.IsTrue(version.Minor >= 0);
        	Assert.IsTrue(version.PatchLevel >= 0);
        	
        	Assert.IsTrue(version.toInt() >= 13000);
        	
        	Regex re = new Regex(@"^\d+\.\d+\.\d+.*$");
        	Assert.IsTrue(re.IsMatch(version.Version));
        }
        
        public void Dispose()
        {
        	    Database.DeleteTestDatabase(Database.TestDatabaseGeneral);
        }
    }
}
