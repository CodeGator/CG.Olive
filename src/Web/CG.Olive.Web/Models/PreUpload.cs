using Microsoft.AspNetCore.Components.Forms;
using System;

namespace CG.Olive.Web.Models
{
    /// <summary>
    /// This class is a model that represents an unsaved upload.
    /// </summary>
    public class PreUpload
    {
        // *******************************************************************
        // Properties.
        // *******************************************************************

        #region Properties

        /// <summary>
        /// This property contains information about a file to be uploaded.
        /// </summary>
        public IBrowserFile File { get; set; }

        /// <summary>
        /// This property contains an application for an upload.
        /// </summary>
        public CG.Olive.Models.Application Application { get; set; }

        /// <summary>
        /// This property contains an environment for an upload.
        /// </summary>
        public CG.Olive.Models.Environment Environment { get; set; }

        #endregion
    }

}
