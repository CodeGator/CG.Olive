using CG.Options;
using System;
using System.ComponentModel.DataAnnotations;

namespace CG.Olive.Web.Options
{
    /// <summary>
    /// This class contains configuration settings for authenticating the
    /// internal REST API.
    /// </summary>
    public class ApiAuthOptions : OptionsBase
    {
        // *******************************************************************
        // Properties.
        // *******************************************************************

        #region Properties

        /// <summary>
        /// This property contains the name for an API.
        /// </summary>
        [Required]
        public string ApiName { get; set; }

        /// <summary>
        /// This property contains a url to an identity authority.
        /// </summary>
        [Required]
        public string Authority { get; set; }

        /// <summary>
        /// This property contains a client secret for the API.
        /// </summary>
        [Required]
        public string ApiSecret { get; set; }

        #endregion
    }
}
