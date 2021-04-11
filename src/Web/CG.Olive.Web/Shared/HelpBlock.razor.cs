using Microsoft.AspNetCore.Components;
using System;

namespace CG.Olive.Web.Shared
{
    /// <summary>
    /// This class is the code-behind for the <see cref="HelpBlock"/> component.
    /// </summary>
    public partial class HelpBlock
    {
        // *******************************************************************
        // Properties.
        // *******************************************************************

        #region Properties

        /// <summary>
        /// This property indicated whether or not the block is visible.
        /// </summary>
        [Parameter]
        public bool Visible { get; set; }

        /// <summary>
        /// This property contains the content for the block.
        /// </summary>
        [Parameter]
        public string Content { get; set; }

        /// <summary>
        /// This property contains child content for the block.
        /// </summary>
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
