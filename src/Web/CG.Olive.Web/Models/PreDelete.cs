using System;
using System.ComponentModel.DataAnnotations;

namespace CG.Olive.Web.Models
{
    /// <summary>
    /// This class is a model that represents a soon to be deleted upload.
    /// </summary>
    public class PreDelete
    {
        // *******************************************************************
        // Properties.
        // *******************************************************************

        #region Properties

        /// <summary>
        /// This property contains the name of the model to be deleted.
        /// </summary>
        public string ModelName { get; set; }

        /// <summary>
        /// This property contains a count of the number of children that 
        /// will be impacted by this operation.
        /// </summary>
        public long ChildCount { get; set; }

        /// <summary>
        /// This property contains the verified input from the UI.
        /// </summary>
        [Required(ErrorMessage ="The file name is required!") ]
        [Compare("ModelName", ErrorMessage ="The file names must match!")]
        public string VerifiedName { get; set; }

        #endregion
    }
}
