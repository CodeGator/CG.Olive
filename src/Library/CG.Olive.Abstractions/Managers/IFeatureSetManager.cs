using CG.Business.Managers;
using CG.Olive.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CG.Olive.Managers
{
    /// <summary>
    /// This interface represents an object that manages business 
    /// logic related to sets of <see cref="Feature"/> objects.
    /// </summary>
    public interface IFeatureSetManager : IManager
    {
        /// <summary>
        /// This method returns the effective feature set for the specified application 
        /// and environment.
        /// </summary>
        /// <param name="sid">The application security identifier to use for the operation.</param>
        /// <param name="skey">The application security key to use for the operation.</param>
        /// <param name="environment">The optional environment name for the operation.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A task to perform the operation that returns an array of key-value pairs 
        /// that together, represent the feature set for the specified application and environment.</returns>
        Task<KeyValuePair<string, bool>[]> GetAsync(
            string sid,
            string skey,
            string environment,
            CancellationToken cancellationToken = default
            );
    }
}
