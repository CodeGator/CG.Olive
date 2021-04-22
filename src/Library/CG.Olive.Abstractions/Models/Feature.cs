using System.ComponentModel.DataAnnotations;

namespace CG.Olive.Models
{
    /// <summary>
    /// This class represents an application feature.
    /// </summary>
    public class Feature : AuditedModelBase
    {
        // *******************************************************************
        // Properties.
        // *******************************************************************

        #region Properties

        /// <summary>
        /// This property contains the key for this feature.
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string Key { get; set; }

        /// <summary>
        /// This property contains the value for this feature.
        /// </summary>
        [Required]
        public bool Value { get; set; }

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
        /// This property indicates whether the feature is enabled, or not.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// This property contains an optional comment for this feature.
        /// </summary>
        [MaxLength(255)]
        public string Comment { get; set; }

        #endregion
    }
}
