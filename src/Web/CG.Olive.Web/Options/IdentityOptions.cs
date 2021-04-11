using CG.Options;
using System;
using System.ComponentModel.DataAnnotations;

namespace CG.Olive.Web.Options
{
    /// <summary>
    /// This class represents configuration options for OIDC identity.
    /// </summary>
    public class IdentityOptions : OptionsBase
    {
        // *******************************************************************
        // Properties.
        // *******************************************************************

        #region Properties

        /// <summary>
        /// This property contains configuration settings for authenticating the 
        /// internal REST API.
        /// </summary>
        [Required]
        public ApiAuthOptions API { get; set; }

        /// <summary>
        /// This property contains configuration settings for authenticating the 
        /// website via OIDC.
        /// </summary>
        [Required]
        public OidcAuthOptions OIDC { get; set; }

        #endregion
    }
}
