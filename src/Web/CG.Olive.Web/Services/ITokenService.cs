using System;
using System.Threading;
using System.Threading.Tasks;

namespace CG.Olive.Web.Services
{
    /// <summary>
    /// This interface represents an object that manages access tokens.
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        /// This method ensures the returned access token is always current.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The refreshed HTTP access token.</returns>
        Task<string> EnsureAccessTokenAsync(
            CancellationToken cancellationToken = default
            );
    }
}
