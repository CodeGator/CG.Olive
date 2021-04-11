using System;

namespace CG.Olive.Web.Services
{
    /// <summary>
    /// This class provides access to important HTTP related tokens.
    /// </summary>
    public class TokenProvider
    {
        // *******************************************************************
        // Properties.
        // *******************************************************************

        #region Properties

        /// <summary>
        /// This property contains a cross-reference forgery validation token.
        /// </summary>
        public string XsrfToken { get; set; }

        /// <summary>
        /// This property contains a bearer access token.
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// This property contains a refresh token.
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// This property indicates how long until the access token expires.
        /// </summary>
        public DateTimeOffset ExpiresAt { get; set; }

        #endregion
    }
}
