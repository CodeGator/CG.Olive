using CG.Options;
using System;
using System.ComponentModel.DataAnnotations;

namespace CG.Olive.Web.Options
{
    /// <summary>
    /// This class contains configuration settings for authenticating the
    /// website through OIDC.
    /// </summary>
    public class OidcAuthOptions : OptionsBase
    {
        // *******************************************************************
        // Properties.
        // *******************************************************************

        #region Properties

        /// <summary>
        /// This property contains a client identifier for the website.
        /// </summary>
        [Required]
        public string ClientId { get; set; }

        /// <summary>
        /// This property contains a client secret for the website.
        /// </summary>
        [Required]
        public string ClientSecret { get; set; }

        #endregion
    }
}
