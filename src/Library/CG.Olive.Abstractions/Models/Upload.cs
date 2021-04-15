using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CG.Olive.Models
{
    /// <summary>
    /// This class is a model that represents a configuration upload. 
    /// </summary>
    public class Upload : AuditedModelBase
    {
        // *******************************************************************
        // Properties.
        // *******************************************************************

        #region Properties

        /// <summary>
        /// This property contains the original file name for the upload.
        /// </summary>
        [Required]
        [MaxLength(260)]
        public string FileName { get; set; }

        /// <summary>
        /// This property contains the application associated with the upload.
        /// </summary>
        public int ApplicationId { get; set; }

        /// <summary>
        /// This property contains the associated application.
        /// </summary>
        [Required]
        public virtual Application Application { get; set; }

        /// <summary>
        /// This property identifies the environment associated with the upload.
        /// </summary>
        public int EnvironmentId { get; set; }

        /// <summary>
        /// This property contains the environment associated with the upload.
        /// </summary>
        public virtual Environment Environment { get; set; }

        /// <summary>
        /// This property contains the JSON for the upload.
        /// </summary>
        [Required]
        public string Json { get; set; }

        /// <summary>
        /// This property contains the size of the JSON upload.
        /// </summary>
        public long Size { get; set; }

        #endregion
    }
}
