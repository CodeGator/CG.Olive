using CG.Business.Stores;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CG.Olive.Stores
{
    /// <summary>
    /// This interface represents an object that manages configurations for 
    /// external clients.
    /// </summary>
    public interface IConfigurationStore : IStore
    {
        /// <summary>
        /// This method returns the effective configuration for the specified 
        /// application and environment.
        /// </summary>
        /// <param name="sid">The application security identifier to use for the operation.</param>
        /// <param name="skey">The application security key to use for the operation.</param>
        /// <param name="environment">The optional environment name for the operation.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A task to perform the operation that returns an enumerable sequence
        /// of key-value pairs, that represent the configuration for the specified 
        /// application and environment.</returns>
        Task<IEnumerable<KeyValuePair<string, string>>> GetAsync(
            string sid,
            string skey,
            string environment,
            CancellationToken cancellationToken = default
            );
    }
}
