using CG.Business.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CG.Olive.Models
{
    /// <summary>
    /// This class represents a configuration setting.
    /// </summary>
    public class Setting : ModelBase
    {
        // *******************************************************************
        // Properties.
        // *******************************************************************

        #region Properties

        /// <summary>
        /// This property is the unique identifier for the setting.
        /// </summary>
        public int Id { get; set; }

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
        /// This property contains the date when the setting was created.
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// This property contains the name of the person who created the setting.
        /// </summary>
        [MaxLength(50)]
        public string CreatedBy { get; set; }

        /// <summary>
        /// This property contains the date when the setting was last updated.
        /// </summary>
        public DateTime? UpdatedDate { get; set; }

        /// <summary>
        /// This property contains the name of the person who last updated the setting.
        /// </summary>
        [MaxLength(50)]
        public string UpdatedBy { get; set; }

        #endregion

        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <summary>
        /// This method returns a hashcode for the object.
        /// </summary>
        /// <returns>A hashcode for the object.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode() ^ Id.GetHashCode();
        }

        // *******************************************************************

        /// <summary>
        /// This method is overriden in order to determine equality.
        /// </summary>
        /// <param name="obj">The model to compare with.</param>
        /// <returns>True if the objects are equal; false otherwise.</returns>
        public override bool Equals(object obj)
        {
            // If the parameter is null, can't be equal.
            if (null == obj)
            {
                return false;
            }

            // If the types don't match, can't be equal.
            if (GetType() != obj.GetType())
            {
                return false;
            }

            // Return an equality comparison of the key properties.
            return EqualityComparer<int>.Default.Equals(
                Id,
                (obj as Setting).Id
                );
        }

        // *******************************************************************

        /// <summary>
        /// This method returns a string that represents the current model.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            // Return a string representation of the object.
            return $"{base.ToString()} - Id: {Id}";
        }

        #endregion
    }
}
