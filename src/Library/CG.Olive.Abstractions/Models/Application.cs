using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CG.Olive.Models
{
    /// <summary>
    /// This class is a model that represents a configuration application. 
    /// </summary>
    public class Application : AuditedModelBase
    {
        // *******************************************************************
        // Properties.
        // *******************************************************************

        #region Properties

        /// <summary>
        /// This property contains the name for the application.
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// This property indicates whether the application is locked, or not.
        /// </summary>
        public bool IsLocked { get; set; }

        /// <summary>
        /// This property contains the security identifier for the application.
        /// </summary>
        [Required]
        public string Sid { get; set; }

        /// <summary>
        /// This property contains the security key for the application.
        /// </summary>
        [Required]
        public string SKey { get; set; }

        #endregion

        // *******************************************************************
        // Constructors.
        // *******************************************************************

        #region Constructors

        /// <summary>
        /// This constructor creates a new instance of the <see cref="Application"/>
        /// class.
        /// </summary>
        public Application()
        {
            // Create defaults for the security stuff (user can change, later).
            Sid = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            SKey = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        }

        #endregion
    }
}
