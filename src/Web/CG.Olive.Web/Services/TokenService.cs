using CG.Olive.Web.Options;
using CG.Validations;
using IdentityModel.Client;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CG.Olive.Web.Services
{
    /// <summary>
    /// This class provides access to always refreshed HTTP related tokens.
    /// </summary>
    public class TokenService : ITokenService
    {
        // *******************************************************************
        // Fields.
        // *******************************************************************

        #region Fields

        /// <summary>
        /// This field contains the cached HTTP tokens.
        /// </summary>
        private readonly TokenProvider _tokenProvider;

        /// <summary>
        /// This field contains a factory for creating HTTP client objects.
        /// </summary>
        private readonly IHttpClientFactory _httpClientFactory;

        /// <summary>
        /// This field contains options related to the identity server.
        /// </summary>
        private readonly IOptions<IdentityOptions> _identityOptions;

        #endregion

        // *******************************************************************
        // Constructors.
        // *******************************************************************

        #region Constructors

        /// <summary>
        /// This constructor creates a new instance of the <see cref="TokenService"/>
        /// class.
        /// </summary>
        /// <param name="tokenProvider">The token provider to use with the service.</param>
        /// <param name="httpClientFactory">The HTTP client factory to use with 
        /// the service.</param>
        /// <param name="identityOptions">The identity options to use with the service.</param>
        public TokenService(
            TokenProvider tokenProvider,
            IHttpClientFactory httpClientFactory,
            IOptions<IdentityOptions> identityOptions
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(tokenProvider, nameof(tokenProvider))
                .ThrowIfNull(httpClientFactory, nameof(httpClientFactory))
                .ThrowIfNull(identityOptions, nameof(identityOptions));

            // Save the references.
            _tokenProvider = tokenProvider;
            _httpClientFactory = httpClientFactory;
            _identityOptions = identityOptions;
        }

        #endregion

        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <inheritdoc />
        public virtual async Task<string> EnsureAccessTokenAsync(
            CancellationToken cancellationToken = default
            )
        {
            // Is the current access token valid?
            if (false == string.IsNullOrEmpty(_tokenProvider.AccessToken) && 
                _tokenProvider.ExpiresAt.AddSeconds(-60).ToUniversalTime() > DateTime.UtcNow)
            {
                // Return the existing token.
                return _tokenProvider.AccessToken;
            }

            // Are we missing a refresh token?
            if (string.IsNullOrEmpty(_tokenProvider.RefreshToken))
            {
                // Just return whatever we have, we can't refresh the access token.
                return _tokenProvider.AccessToken;
            }

            // If we get here then we either never had a valid token to begin with, or,
            //   enough time has gone by, since we first created the token, that we now
            //   need to refresh it again.

            // Create a new client. (Note, we don't employ a using block here because,
            //   in a server environment like this one, the HttpClient object is actually
            //   shared so we don't run out of available sockets.)
            var httpClient = _httpClientFactory.CreateClient();

            // Look for a disco document on the server.
            var discoReponse = await httpClient.GetDiscoveryDocumentAsync(
                _identityOptions.Value.Authority,
                cancellationToken
                );

            // Did we fail?
            if (discoReponse.IsError)
            {
                // Panic!
                throw new Exception(
                    message: $"Unable to locate a disco doc. " +
                    $"error: {discoReponse.Error}"
                    ).SetCallerInfo()
                     .SetOriginator(nameof(TokenService))
                     .SetDateTime();
            }

            // Ask for an updated access token for this client ('this client' = us).
            var refreshResponse = await httpClient.RequestRefreshTokenAsync(
               new RefreshTokenRequest
               {
                   Address = discoReponse.TokenEndpoint,
                   ClientId = _identityOptions.Value.OIDC.ClientId,
                   ClientSecret = _identityOptions.Value.OIDC.ClientSecret,
                   RefreshToken = _tokenProvider.RefreshToken
               }, 
               cancellationToken
               );

            // Did we fail?
            if (refreshResponse.IsError)
            {
                // Panic!
                throw new Exception(
                    message: $"Unable to refresh the access token. " +
                    $"error: {refreshResponse.Error}"
                    ).SetCallerInfo()
                     .SetOriginator(nameof(TokenService))
                     .SetDateTime();
            }

            // Update the values in the token provider.
            _tokenProvider.AccessToken = refreshResponse.AccessToken;
            _tokenProvider.RefreshToken = refreshResponse.RefreshToken;
            _tokenProvider.ExpiresAt = DateTime.UtcNow.AddSeconds(refreshResponse.ExpiresIn);

            // Return the newly refreshed access token.
            return _tokenProvider.AccessToken;
        }

        #endregion

    }
}
