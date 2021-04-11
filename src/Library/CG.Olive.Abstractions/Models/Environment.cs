using CG.Business.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CG.Olive.Models
{
    /// <summary>
    /// This class is a model that represents a configuration environment. 
    /// </summary>
    public class Environment : ModelBase
    {
        // *******************************************************************
        // Properties.
        // *******************************************************************

        #region Properties

        /// <summary>
        /// This property contains a unique identifier for the environment.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// This property contains the name for the environment.
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// This property indicates whether this environment is the default one.
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// This property contains the date when the environment was created.
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// This property contains the name of the user who created the environment.
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// This property contains the date when the environment was last modified.
        /// </summary>
        public DateTime? UpdatedDate { get; set; }

        /// <summary>
        /// This property contains the name of the user who last environment the upload.
        /// </summary>
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
                (obj as Environment).Id
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
