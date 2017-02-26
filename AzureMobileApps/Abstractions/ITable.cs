namespace Microsoft.Azure.Mobile.Core.Server.Abstractions
{
    /// <summary>
    /// Describes the form of an Azure Mobile Apps table configuration.
    /// </summary>
    public interface ITable
    {
        /// <summary>
        /// The name of the endpoint - will appear under /tables/{Name} normally
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The domain manager for this endpoint
        /// </summary>
        IDomainManager DomainManager { get; }

        /// <summary>
        /// The authorization filter for this endpoint (null if not set)
        /// </summary>
        IMobileTableAuthorization Authorization { get; set; }

        /// <summary>
        /// The filter for this endpoint (null if not set)
        /// </summary>
        IMobileTableFilter Filter { get; set; }

        /// <summary>
        /// The transform for this endpoint (null if not set)
        /// </summary>
        IMobileTableTransform Transform { get; set; }

        /// <summary>
        /// The action for this endpoint (null if not set)
        /// </summary>
        IMobileTableAction Action { get; set; }
    }
}
