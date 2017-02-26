using Microsoft.Azure.Mobile.Core.Server.Managers;
using Microsoft.Azure.Mobile.Core.Server.Tables;
using System;
using Xunit;

namespace AzureMobileApps.Tests.Tables
{
    public class MobileTableTests
    {
        [Fact]
        public void MobileTable_Throws_ArgumentNullException_NameArg()
        {
            Assert.Throws<ArgumentNullException>(
                () => { var x = new MobileTable(null, new InMemoryDomainManager()); });
        }

        [Fact]
        public void MobileTable_Throws_ArgumentNullException_DomainArg()
        {
            Assert.Throws<ArgumentNullException>(
                () => { var x = new MobileTable("test", null); });
        }
    }
}
