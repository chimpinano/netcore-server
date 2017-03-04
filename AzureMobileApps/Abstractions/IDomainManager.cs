using Newtonsoft.Json.Linq;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Azure.Mobile.Core.Server.Abstractions
{
    /// <summary>
    /// Required interface for implementing a Domain Manager for handling data interface
    /// </summary>
    public interface IDomainManager
    {
        /// <summary>
        /// Retrieves an object within the data provider.  
        /// </summary>
        /// <param name="item">The item to be retrieved with only the necessary information provided</param>
        /// <returns>The object that was retrieved</returns>
        /// <exception cref="DomainManagerObjectNotFoundException">If the object is not found in the data provider</exception>
        /// <exception cref="DomainManagerMalformedObject">If the object passed in is malformed</exception>
        Task<JObject> GetObjectAsync(JObject item);

        /// <summary>
        /// Retrieves an <see cref="IQueryable{JObject}"/> for the data provider for searching capabilities
        /// </summary>
        /// <returns>An <see cref="IQueryable{JObject}"/> for the data provider</returns>
        Task<IQueryable<JObject>> GetObjectsAsync();

        /// <summary>
        /// Inserts an object within the data provider.  
        /// </summary>
        /// <param name="item">The item to be inserted</param>
        /// <returns>The object that was actually inserted</returns>
        /// <exception cref="DomainManagerObjectExistsException">If the object already exists in the data provider</exception>
        /// <exception cref="DomainManagerMalformedObject">If the object passed in is malformed</exception>
        Task<JObject> InsertObjectAsync(JObject item);

        /// <summary>
        /// Update an object within the data provider, merging the provided object with the existing object.  
        /// </summary>
        /// <param name="item">The item to be merged</param>
        /// <returns>The updated item</returns>
        /// <exception cref="DomainManagerObjectNotFoundException">If the object is not found in the data provider</exception>
        /// <exception cref="DomainManagerMalformedObject">If the object passed in is malformed</exception>
        /// <exception cref="DomainManagerConflictException">If the version does not match what exists</exception>
        Task<JObject> UpdateObjectAsync(JObject item);

        /// <summary>
        /// Replace an object within the data provider.  
        /// </summary>
        /// <param name="item">The new version of the item</param>
        /// <returns>The new item</returns>
        /// <exception cref="DomainManagerObjectNotFoundException">If the object is not found in the data provider</exception>
        /// <exception cref="DomainManagerMalformedObject">If the object passed in is malformed</exception>
        /// <exception cref="DomainManagerConflictException">If the version does not match what exists</exception>
        Task<JObject> ReplaceObjectAsync(JObject item);

        /// <summary>
        /// Deletes an object within the data provider.  
        /// </summary>
        /// <param name="item">The item to be deleted</param>
        /// <returns>The object that was deleted</returns>
        /// <exception cref="DomainManagerObjectNotFoundException">If the object is not found in the data provider</exception>
        /// <exception cref="DomainManagerMalformedObject">If the object passed in is malformed</exception>
        Task<JObject> DeleteObjectAsync(JObject item);
    }
}
