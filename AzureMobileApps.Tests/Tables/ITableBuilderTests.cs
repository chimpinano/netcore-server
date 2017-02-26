using Microsoft.Azure.Mobile.Core.Server.Abstractions;
using Microsoft.Azure.Mobile.Core.Server.Managers;
using Microsoft.Azure.Mobile.Core.Server.Tables;
using Xunit;

namespace AzureMobileApps.Tests.Tables
{
    public class ITableBuilderTests
    {
        [Fact]
        public void Can_CreateITableBuilder()
        {
            ITableBuilder t = new MobileTableBuilder();
            Assert.NotNull(t);
        }

        [Fact]
        public void New_ITableBuilder_Is_Empty()
        {
            ITableBuilder t = new MobileTableBuilder();
            Assert.Equal(0, t.Tables.Count);
        }

        [Fact]
        public void Can_AddTable_To_ITableBuilder()
        {
            ITableBuilder t = new MobileTableBuilder();
            t.AddTable("todoitem", new InMemoryDomainManager());
            Assert.Equal(1, t.Tables.Count);
            Assert.Equal("todoitem", t.Tables[0].Name);
        }
    }
}
