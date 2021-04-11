using IdentityModel.Client;
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
        public TokenService(
            TokenProvider tokenProvider,
            IHttpClientFactory httpClientFactory
            )
        {
            // Save the references.
            _tokenProvider = tokenProvider;
            _httpClientFactory = httpClientFactory;
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
                "https://localhost:5001",
                cancellationToken
                );

            // Did we fail?
            if (discoReponse.IsError)
            {
                // Panic!
                throw new Exception(
                    message: $"{nameof(TokenService)} error. Unable to locate a disco doc. " +
                    $"error: {discoReponse.Error}"
                    );
            }

            // Ask for an updated access token for this client ('this client' = us).
            var refreshResponse = await httpClient.RequestRefreshTokenAsync(
               new RefreshTokenRequest
               {
                   Address = discoReponse.TokenEndpoint,
                   ClientId = "DA689B8E-6B09-47DE-8328-CFFB911CE9D7",
                   ClientSecret = "3DFC0E24-A141-4181-BFAD-10ADF2709550",
                   RefreshToken = _tokenProvider.RefreshToken
               }, 
               cancellationToken
               );

            // Did we fail?
            if (refreshResponse.IsError)
            {
                // Panic!
                throw new Exception(
                    message: $"{nameof(TokenService)} error. Unable to refresh the access token. " +
                    $"error: {refreshResponse.Error}"
                    );
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
