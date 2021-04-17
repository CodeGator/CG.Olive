using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CG.Olive.Models
{
    /// <summary>
    /// This class represents a configuration setting.
    /// </summary>
    public class Setting : AuditedModelBase
    {
        // *******************************************************************
        // Properties.
        // *******************************************************************

        #region Properties

        /// <summary>
        /// This property contains an identifier for the associated upload
        /// for the setting.
        /// </summary>
        public int UploadId { get; set; }

        /// <summary>
        /// This property contains a reference to the associatred upload for
        /// the setting.
        /// </summary>
        public virtual Upload Upload { get; set; }

        /// <summary>
        /// This property contains the key for this setting.
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string Key { get; set; }

        /// <summary>
        /// This property contains an optional environment identifier for 
        /// this setting.
        /// </summary>
        public int EnvironmentId { get; set; }

        /// <summary>
        /// This property contains a reference to the associated environment for 
        /// this setting.
        /// </summary>
        public virtual Environment Environment { get; set; }

        /// <summary>
        /// This property contains an identifier for the associated application
        /// for this setting.
        /// </summary>
        public int ApplicationId { get; set; }

        /// <summary>
        /// This property contains a reference to the associated application for 
        /// this setting.
        /// </summary>
        public virtual Application Application { get; set; }

        /// <summary>
        /// This property contains the value for this setting.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// This property contains an optional comment for this setting.
        /// </summary>
        [MaxLength(255)]
        public string Comment { get; set; }

        /// <summary>
        /// This property indicates the value should be overridden from 
        /// secret storage.
        /// </summary>
        public bool IsSecret { get; set; }

        #endregion
    }
}
