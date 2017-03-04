using System;
using System.Collections.Generic;
using Microsoft.Azure.Mobile.Core.Server.Abstractions;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Linq;

namespace Microsoft.Azure.Mobile.Core.Server.Managers
{
    public class InMemoryDomainManager : IDomainManager
    {
        private Dictionary<string,JObject> items = new Dictionary<string,JObject>();

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        /// <summary>
        /// Deletes an object within the data provider.  
        /// </summary>
        /// <param name="item">The item to be deleted (the SystemFields.IdField field must be specified)</param>
        /// <returns>The object that was deleted</returns>
        /// <exception cref="DomainManagerObjectNotFoundException">If the object is not found in the data provider</exception>
        /// <exception cref="DomainManagerMalformedObject">If the object passed in is malformed</exception>
        public async Task<JObject> DeleteObjectAsync(JObject item)
        {
            var key = EnsureIdExists(item);
            var obj = items[key];
            items.Remove(key);
            return obj;
        }

        /// <summary>
        /// Retrieves an object within the data provider.  The provided object must have an SystemFields.IdField field 
        /// </summary>
        /// <param name="item">The item to be retrieved with only the SystemFields.IdField field specified</param>
        /// <returns>The object that was retrieved</returns>
        /// <exception cref="DomainManagerObjectNotFoundException">If the object is not found in the data provider</exception>
        /// <exception cref="DomainManagerMalformedObject">If the object passed in is malformed</exception>
        public async Task<JObject> GetObjectAsync(JObject item)
        {
            var key = EnsureIdExists(item);
            return items[key];
        }

        /// <summary>
        /// Retrieves an <see cref="IQueryable{JObject}"/> for the data provider for searching capabilities
        /// </summary>
        /// <returns>An <see cref="IQueryable{JObject}"/> for the data provider</returns>
        public async Task<IQueryable<JObject>> GetObjectsAsync()
        {
            return items.Values.AsQueryable<JObject>();
        }

        /// <summary>
        /// Inserts an object within the data provider.  
        /// </summary>
        /// <param name="item">The item to be inserted</param>
        /// <returns>The object that was actually inserted</returns>
        /// <exception cref="DomainManagerObjectExistsException">If the object already exists in the data provider</exception>
        /// <exception cref="DomainManagerMalformedObject">If the object passed in is malformed</exception>
        public async Task<JObject> InsertObjectAsync(JObject item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            var newItem = (JObject)item.DeepClone();
            if (newItem[SystemFields.IdField] == null)
            {
                newItem[SystemFields.IdField] = Guid.NewGuid().ToString("N");
            }
            if (items.ContainsKey(newItem[SystemFields.IdField].ToString()))
            {
                throw new DomainManagerObjectExistsException();
            }
            newItem[SystemFields.UpdatedAtField] = DateTimeOffset.UtcNow.ToString("o");
            newItem[SystemFields.VersionField] = ConvertNumericVersion(1);
            items.Add(newItem[SystemFields.IdField].ToString(), newItem);
            return newItem;
        }


        /// <summary>
        /// Update an object within the data provider, merging the provided object with the existing object.  
        /// </summary>
        /// <param name="item">The item to be merged</param>
        /// <returns>The updated item</returns>
        /// <exception cref="DomainManagerObjectNotFoundException">If the object is not found in the data provider</exception>
        /// <exception cref="DomainManagerMalformedObject">If the object passed in is malformed</exception>
        public async Task<JObject> UpdateObjectAsync(JObject item)
        {
            var key = EnsureIdExists(item);
            var version = (string)items[key][SystemFields.VersionField];
            if (item[SystemFields.VersionField] != null && !(item[SystemFields.VersionField].ToString().Equals(version)))
            {
                throw new DomainManagerConflictException();
            }
            var newItem = (JObject)items[key].DeepClone();
            newItem.Merge(item);
            newItem[SystemFields.UpdatedAtField] = DateTimeOffset.UtcNow.ToString("o");
            long oldVersion = newItem[SystemFields.VersionField] != null ? 
                ConvertBase64Version(newItem[SystemFields.VersionField].ToString()) + 1 : 1;
            newItem[SystemFields.VersionField] = ConvertNumericVersion(oldVersion);
            items[key] = newItem;
            return newItem;
        }

        /// <summary>
        /// Replace an object within the data provider.  
        /// </summary>
        /// <param name="item">The new version of the item</param>
        /// <returns>The new item</returns>
        /// <exception cref="DomainManagerObjectNotFoundException">If the object is not found in the data provider</exception>
        /// <exception cref="DomainManagerMalformedObject">If the object passed in is malformed</exception>
        /// <exception cref="DomainManagerConflictException">If there is a conflict from versions</exception>
        public async Task<JObject> ReplaceObjectAsync(JObject item)
        {
            var key = EnsureIdExists(item);
            var version = (string)items[key][SystemFields.VersionField];
            if (item[SystemFields.VersionField] != null && !(item[SystemFields.VersionField].ToString().Equals(version)))
            {
                throw new DomainManagerConflictException();
            }
            var newItem = (JObject)item.DeepClone();
            newItem[SystemFields.UpdatedAtField] = DateTimeOffset.UtcNow.ToString("o");
            long oldVersion = newItem[SystemFields.VersionField] != null ? 
                ConvertBase64Version(newItem[SystemFields.VersionField].ToString()) + 1 : 1;
            newItem[SystemFields.VersionField] = ConvertNumericVersion(oldVersion);
            items[key] = newItem;
            return newItem;
        }

#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        /// <summary>
        /// Private method for repeated functionality - ensure the key exists and return it if it does
        /// </summary>
        /// <param name="item">The item to check</param>
        /// <returns>The id of the item</returns>
        /// <exception cref="DomainManagerObjectNotFoundException">if the ID is not found in the domain</exception>
        /// <exception cref="DomainManagerMalformedObject">if the object passed in doesn't have an SystemFields.IdField field</exception>
        private string EnsureIdExists(JObject item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            if (item[SystemFields.IdField] == null)
            {
                throw new DomainManagerMalformedObject();
            }
            var key = item[SystemFields.IdField].ToString();
            if (!items.ContainsKey(key))
            {
                throw new DomainManagerObjectNotFoundException();
            }
            return key;
        }

        /// <summary>
        /// Converts a number into a version string for storage in the object
        /// </summary>
        /// <param name="v">The new version as an long integer</param>
        /// <returns>The JToken for the version</returns>
        private string ConvertNumericVersion(long v)
            => Convert.ToBase64String(BitConverter.GetBytes(v));

        /// <summary>
        /// Converts a base-64 encoded version string into a long for manipulation
        /// </summary>
        /// <param name=SystemFields.VersionField>The base-64 encoded version string</param>
        /// <returns>The long integer version value</returns>
        private long ConvertBase64Version(string version)
            => BitConverter.ToInt64(Convert.FromBase64String(version), 0);
    }
}
