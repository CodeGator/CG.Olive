using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

using Feature = CG.Olive.Models.Feature;

namespace CG.Olive.Web.Pages.Features
{
    /// <summary>
    /// This class is the code-behind for the <see cref="EditDialog"/> razor view.
    /// </summary>
    public partial class EditDialog
    {
        // *******************************************************************
        // Properties.
        // *******************************************************************

        #region Properties

        /// <summary>
        /// This property contains a reference to the dialog.
        /// </summary>
        [CascadingParameter]
        MudDialogInstance MudDialog { get; set; }

        /// <summary>
        /// This property contains a reference to the model.
        /// </summary>
        [Parameter]
        public Feature Model { get; set; }

        #endregion

        // *******************************************************************
        // Private methods.
        // *******************************************************************

        #region Private methods

        /// <summary>
        /// This method is called whenever the user cancels the dialog.
        /// </summary>
        private void Cancel()
        {
            MudDialog.Cancel();
        }

        // *******************************************************************

        /// <summary>
        /// This method is called whenever the user submits the dialog.
        /// </summary>
        /// <param name="context">The edit context to use for the operation.</param>
        private void OnValidSubmit(EditContext context)
        {
            if (context.Validate())
            {
                MudDialog.Close(DialogResult.Ok(Model.Id));
            }
        }

        #endregion
    }
}
