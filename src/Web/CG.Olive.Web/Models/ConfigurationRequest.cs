using System;
using System.ComponentModel.DataAnnotations;

namespace CG.Olive.Web.Models
{
    /// <summary>
    /// This class is a model for incoming configuration requests.
    /// </summary>
    public class ConfigurationRequest 
    {
        // *******************************************************************
        // Properties.
        // *******************************************************************

        #region Properties

        /// <summary>
        /// This property contains the security identifier for the request.
        /// </summary>
        [Required]
        public string Sid { get; set; }

        /// <summary>
        /// This property contains the security key for the request.
        /// </summary>
        [Required]
        public string SKey { get; set; }

        /// <summary>
        /// This property contains an optional environment for the request.
        /// </summary>
        public string Environment { get; set; }

        #endregion
    }
}
