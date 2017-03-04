using Microsoft.Azure.Mobile.Core.Server.Managers;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AzureMobileApps.Tests.Managers
{
    public class InMemoryDomainManagerTests
    {
        [Fact]
        public async Task Creates_Empty_Provider_Async()
        {
            var provider = new InMemoryDomainManager();
            var queryable = await provider.GetObjectsAsync();
            Assert.Equal(0, queryable.Count());
        }

        [Fact]
        public async Task Insert_Throws_Null_Async()
        {
            var provider = new InMemoryDomainManager();
            await Assert.ThrowsAsync<ArgumentNullException>(() => provider.InsertObjectAsync(null));
        }

        [Fact]
        public async Task Can_Insert_NoId_Async()
        {
            var provider = new InMemoryDomainManager();
            var item = new JObject();
            item["test"] = "foo";
            var obj = await provider.InsertObjectAsync(item);
            Assert.NotNull(obj);
            Assert.NotNull(obj["id"]);
            Assert.NotNull(obj["updatedAt"]);
            Assert.NotNull(obj["version"]);
            Assert.Equal("foo", obj["test"].ToString());
        }

        [Fact]
        public async Task Can_Insert_WithId_Async()
        {
            var provider = new InMemoryDomainManager();
            var item = new JObject();
            item["id"] = "test";
            item["test"] = "foo";
            var obj = await provider.InsertObjectAsync(item);
            Assert.NotNull(obj);
            Assert.NotNull(obj["id"]);
            Assert.NotNull(obj["updatedAt"]);
            Assert.NotNull(obj["version"]);
            Assert.Equal("test", obj["id"].ToString());
            Assert.Equal("foo", obj["test"].ToString());
        }

        [Fact]
        public async Task Insert_Doesnt_Alter_Original_Async()
        {
            var provider = new InMemoryDomainManager();
            var item = new JObject();
            item["id"] = "test";
            item["test"] = "foo";
            var obj = await provider.InsertObjectAsync(item);
            Assert.NotNull(obj);
            Assert.Null(item["updatedAt"]);
            Assert.Null(item["version"]);
        }

        [Fact]
        public async Task Delete_Throws_Null_Async()
        {
            var provider = new InMemoryDomainManager();
            await Assert.ThrowsAsync<ArgumentNullException>(() => provider.DeleteObjectAsync(null));
        }

        [Fact]
        public async Task Cannot_Insert_Dupe_Async()
        {
            var provider = new InMemoryDomainManager();
            var item = new JObject();
            item["id"] = "test";
            item["test"] = "foo";
            await provider.InsertObjectAsync(item);

            await Assert.ThrowsAsync<DomainManagerObjectExistsException>(() => provider.InsertObjectAsync(item));
        }

        [Fact]
        public async Task Can_Delete_Async()
        {
            var provider = new InMemoryDomainManager();
            var item = new JObject();
            item["id"] = "test";
            item["test"] = "foo";
            var obj = await provider.InsertObjectAsync(item);
            Assert.NotNull(obj);
            Assert.NotNull(obj["id"]);
            Assert.NotNull(obj["updatedAt"]);
            Assert.NotNull(obj["version"]);
            Assert.Equal("test", obj["id"].ToString());
            Assert.Equal("foo", obj["test"].ToString());

            var queryable = await provider.GetObjectsAsync();
            Assert.Equal(1, queryable.Count());

            await provider.DeleteObjectAsync(item);
            Assert.Equal(0, queryable.Count());
        }

        [Fact]
        public async Task Delete_Invalid_Throws_Async()
        {
            var provider = new InMemoryDomainManager();
            var item = new JObject();

            await Assert.ThrowsAsync<DomainManagerMalformedObject>(() => provider.DeleteObjectAsync(item));
        }

        [Fact]
        public async Task Cannot_Double_Delete_Async()
        {
            var provider = new InMemoryDomainManager();
            var item = new JObject();
            item["id"] = "test";
            item["test"] = "foo";
            var obj = await provider.InsertObjectAsync(item);
            Assert.NotNull(obj);
            Assert.NotNull(obj["id"]);
            Assert.NotNull(obj["updatedAt"]);
            Assert.NotNull(obj["version"]);
            Assert.Equal("test", obj["id"].ToString());
            Assert.Equal("foo", obj["test"].ToString());

            var queryable = await provider.GetObjectsAsync();
            Assert.Equal(1, queryable.Count());

            await provider.DeleteObjectAsync(item);
            Assert.Equal(0, queryable.Count());

            await Assert.ThrowsAsync<DomainManagerObjectNotFoundException>(() => provider.DeleteObjectAsync(item));
        }

        [Fact]
        public async Task GetObject_Null_Throws_Async()
        {
            var provider = new InMemoryDomainManager();
            await Assert.ThrowsAsync<ArgumentNullException>(() => provider.GetObjectAsync(null));
        }

        [Fact]
        public async Task GetObject_NoId_Throws_Async()
        {
            var provider = new InMemoryDomainManager();
            var item = new JObject();
            await Assert.ThrowsAsync<DomainManagerMalformedObject>(() => provider.GetObjectAsync(item));
        }

        [Fact]
        public async Task GetObject_IdDoesNotExist_Throws_Async()
        {
            var provider = new InMemoryDomainManager();
            var item = new JObject();
            item["id"] = "test";
            await Assert.ThrowsAsync<DomainManagerObjectNotFoundException>(() => provider.GetObjectAsync(item));
        }

        [Fact]
        public async Task GetObject_IdExists_Async()
        {
            var provider = new InMemoryDomainManager();
            var item = new JObject();
            item["id"] = "test";
            item["test"] = "foo";
            var obj = await provider.InsertObjectAsync(item);
            Assert.NotNull(obj);
            Assert.NotNull(obj["id"]);
            Assert.NotNull(obj["updatedAt"]);
            Assert.NotNull(obj["version"]);
            Assert.Equal("test", obj["id"].ToString());
            Assert.Equal("foo", obj["test"].ToString());

            var item2 = new JObject();
            item2["id"] = "test";
            var obj2 = await provider.GetObjectAsync(item2);
            Assert.NotNull(obj2);
            Assert.NotNull(obj2["id"]);
            Assert.NotNull(obj2["updatedAt"]);
            Assert.NotNull(obj2["version"]);
            Assert.Equal("test", obj2["id"].ToString());
            Assert.Equal("foo", obj2["test"].ToString());
        }

        [Fact]
        public async Task UpdateObject_Null_Throws_Async()
        {
            var provider = new InMemoryDomainManager();
            await Assert.ThrowsAsync<ArgumentNullException>(() => provider.UpdateObjectAsync(null));
        }

        [Fact]
        public async Task UpdateObject_NoId_Throws_Async()
        {
            var provider = new InMemoryDomainManager();
            var item = new JObject();
            await Assert.ThrowsAsync<DomainManagerMalformedObject>(() => provider.UpdateObjectAsync(item));
        }
        [Fact]
        public async Task UpdateObject_IdDoesNotExist_Throws_Async()
        {
            var provider = new InMemoryDomainManager();
            var item = new JObject();
            item["id"] = "test";
            await Assert.ThrowsAsync<DomainManagerObjectNotFoundException>(() => provider.UpdateObjectAsync(item));
        }

        [Fact]
        public async Task UpdateObject_Merges_Async()
        {
            var provider = new InMemoryDomainManager();
            var item = new JObject();
            item["id"] = "test";
            item["test"] = "foo";
            var obj = await provider.InsertObjectAsync(item);
            Assert.NotNull(obj);
            Assert.NotNull(obj["id"]);
            Assert.NotNull(obj["updatedAt"]);
            Assert.NotNull(obj["version"]);
            Assert.Equal("test", obj["id"].ToString());
            Assert.Equal("foo", obj["test"].ToString());

            var item2 = new JObject();
            item2["id"] = "test";
            item2["test2"] = "foo2";
            var obj2 = await provider.UpdateObjectAsync(item2);
            Assert.NotNull(obj2);
            Assert.NotNull(obj2["id"]);
            Assert.NotNull(obj2["updatedAt"]);
            Assert.NotNull(obj["version"]);
            Assert.Equal("test", obj2["id"].ToString());
            Assert.Equal("foo", obj2["test"].ToString());
            Assert.Equal("foo2", obj2["test2"].ToString());

            var item3 = new JObject();
            item3["id"] = "test";
            item3["test"] = "new";
            var obj3 = await provider.UpdateObjectAsync(item3);
            Assert.NotNull(obj3);
            Assert.NotNull(obj3["id"]);
            Assert.NotNull(obj3["updatedAt"]);
            Assert.NotNull(obj["version"]);
            Assert.Equal("test", obj3["id"].ToString());
            Assert.Equal("new", obj3["test"].ToString());
            Assert.Equal("foo2", obj3["test2"].ToString());
        }
        [Fact]
        public async Task UpdateObject_VersionMatch_Async()
        {
            var provider = new InMemoryDomainManager();
            var item = new JObject();
            item["id"] = "test";
            item["test"] = "foo";
            var obj = await provider.InsertObjectAsync(item);
            Assert.NotNull(obj);
            Assert.NotNull(obj["id"]);
            Assert.NotNull(obj["updatedAt"]);
            Assert.NotNull(obj["version"]);
            Assert.Equal("test", obj["id"].ToString());
            Assert.Equal("foo", obj["test"].ToString());

            var item2 = new JObject();
            item2["id"] = "test";
            item2["test2"] = "foo2";
            item2["version"] = obj["version"];
            var obj2 = await provider.UpdateObjectAsync(item2);
            Assert.NotNull(obj2);
            Assert.NotNull(obj2["id"]);
            Assert.NotNull(obj2["updatedAt"]);
            Assert.NotNull(obj2["version"]);
            Assert.NotEqual(obj2["version"].ToString(), obj["version"].ToString());
            Assert.Equal("test", obj2["id"].ToString());
            Assert.Equal("foo", obj2["test"].ToString());
            Assert.Equal("foo2", obj2["test2"].ToString());

            var item3 = new JObject();
            item3["id"] = "test";
            item3["test"] = "new";
            item3["version"] = obj["version"];
            await Assert.ThrowsAsync<DomainManagerConflictException>(async () => await provider.UpdateObjectAsync(item3));
        }


        [Fact]
        public async Task Update_Doesnt_Alter_Original_Async()
        {
            var provider = new InMemoryDomainManager();
            var item = new JObject();
            item["id"] = "test";
            item["test"] = "foo";
            var obj = await provider.InsertObjectAsync(item);
            Assert.NotNull(obj);
            Assert.NotNull(obj["id"]);
            Assert.NotNull(obj["updatedAt"]);
            Assert.NotNull(obj["version"]);
            Assert.Equal("test", obj["id"].ToString());
            Assert.Equal("foo", obj["test"].ToString());

            var item2 = new JObject();
            item2["id"] = "test";
            item2["test2"] = "foo2";
            var obj2 = await provider.UpdateObjectAsync(item2);
            Assert.NotNull(obj2);
            Assert.Null(item2["updatedAt"]);
        }

        [Fact]
        public async Task ReplaceObject_Null_Throws_Async()
        {
            var provider = new InMemoryDomainManager();
            await Assert.ThrowsAsync<ArgumentNullException>(() => provider.ReplaceObjectAsync(null));
        }

        [Fact]
        public async Task ReplaceObject_NoId_Throws_Async()
        {
            var provider = new InMemoryDomainManager();
            var item = new JObject();
            await Assert.ThrowsAsync<DomainManagerMalformedObject>(() => provider.ReplaceObjectAsync(item));
        }
        [Fact]
        public async Task ReplaceObject_IdDoesNotExist_Throws_Async()
        {
            var provider = new InMemoryDomainManager();
            var item = new JObject();
            item["id"] = "test";
            await Assert.ThrowsAsync<DomainManagerObjectNotFoundException>(() => provider.ReplaceObjectAsync(item));
        }

        [Fact]
        public async Task ReplaceObject_Replaces_Async()
        {
            var provider = new InMemoryDomainManager();
            var item = new JObject();
            item["id"] = "test";
            item["test"] = "foo";
            var obj = await provider.InsertObjectAsync(item);
            Assert.NotNull(obj);
            Assert.NotNull(obj["id"]);
            Assert.NotNull(obj["updatedAt"]);
            Assert.NotNull(obj["version"]);
            Assert.Equal("test", obj["id"].ToString());
            Assert.Equal("foo", obj["test"].ToString());

            var item2 = new JObject();
            item2["id"] = "test";
            item2["test2"] = "foo2";
            item2["version"] = obj["version"];
            var obj2 = await provider.ReplaceObjectAsync(item2);
            Assert.NotNull(obj2);
            Assert.NotNull(obj2["id"]);
            Assert.NotNull(obj2["updatedAt"]);
            Assert.NotNull(obj2["version"]);
            Assert.Equal("test", obj2["id"].ToString());
            Assert.Null(obj2["test"]);
            Assert.Equal("foo2", obj2["test2"].ToString());

            var item3 = new JObject();
            item3["id"] = "test";
            item3["test"] = "new";
            item3["version"] = obj2["version"];
            var obj3 = await provider.ReplaceObjectAsync(item3);
            Assert.NotNull(obj3);
            Assert.NotNull(obj3["id"]);
            Assert.NotNull(obj3["updatedAt"]);
            Assert.NotNull(obj3["version"]);
            Assert.Equal("test", obj3["id"].ToString());
            Assert.Equal("new", obj3["test"].ToString());
            Assert.Null(obj3["test2"]);
        }

        [Fact]
        public async Task ReplaceObject_VersionMatch_Async()
        {
            var provider = new InMemoryDomainManager();
            var item = new JObject();
            item["id"] = "test";
            item["test"] = "foo";
            var obj = await provider.InsertObjectAsync(item);
            Assert.NotNull(obj);
            Assert.NotNull(obj["id"]);
            Assert.NotNull(obj["updatedAt"]);
            Assert.NotNull(obj["version"]);
            Assert.Equal("test", obj["id"].ToString());
            Assert.Equal("foo", obj["test"].ToString());

            var item2 = new JObject();
            item2["id"] = "test";
            item2["test2"] = "foo2";
            item2["version"] = obj["version"];
            var obj2 = await provider.ReplaceObjectAsync(item2);
            Assert.NotNull(obj2);
            Assert.NotNull(obj2["id"]);
            Assert.NotNull(obj2["updatedAt"]);
            Assert.NotNull(obj2["version"]);
            Assert.Equal("test", obj2["id"].ToString());
            Assert.Null(obj2["test"]);
            Assert.Equal("foo2", obj2["test2"].ToString());

            var item3 = new JObject();
            item3["id"] = "test";
            item3["test"] = "new";
            item3["version"] = obj["version"];
            await Assert.ThrowsAsync<DomainManagerConflictException>(async () => await provider.ReplaceObjectAsync(item3));

        }

        [Fact]
        public async Task Replace_Doesnt_Alter_Original_Async()
        {
            var provider = new InMemoryDomainManager();
            var item = new JObject();
            item["id"] = "test";
            item["test"] = "foo";
            var obj = await provider.InsertObjectAsync(item);
            Assert.NotNull(obj);
            Assert.NotNull(obj["id"]);
            Assert.NotNull(obj["updatedAt"]);
            Assert.Equal("test", obj["id"].ToString());
            Assert.Equal("foo", obj["test"].ToString());

            var item2 = new JObject();
            item2["id"] = "test";
            item2["test2"] = "foo2";
            var obj2 = await provider.ReplaceObjectAsync(item2);
            Assert.NotNull(obj2);
            Assert.Null(item2["updatedAt"]);
        }
    }
}
