using System;
using System.ComponentModel.DataAnnotations;

namespace CG.Olive.Models
{
    /// <summary>
    /// This class is a model that represents a configuration environment. 
    /// </summary>
    public class Environment : AuditedModelBase
    {
        // *******************************************************************
        // Properties.
        // *******************************************************************

        #region Properties

        /// <summary>
        /// This property contains the name for the environment.
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// This property indicates whether this environment is the default one.
        /// </summary>
        public bool IsDefault { get; set; }

        #endregion
    }
}
