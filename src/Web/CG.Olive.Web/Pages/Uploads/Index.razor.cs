using CG.Olive.Models;
using CG.Olive.Stores;
using CG.Olive.Web.Models;
using CG.Olive.Web.Pages.Shared;
using CG.Validations;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CG.Olive.Web.Pages.Uploads
{
    /// <summary>
    /// This class is the code-behind for the <see cref="Index"/> razor view.
    /// </summary>
    public partial class Index
    {
        // *******************************************************************
        // Fields.
        // *******************************************************************

        #region Fields

        /// <summary>
        /// This field contains an error message.
        /// </summary>
        private string _error;

        /// <summary>
        /// This field contains an information message.
        /// </summary>
        private string _info;

        /// <summary>
        /// This field contains a reference to the tab control.
        /// </summary>
#pragma warning disable IDE0044  // can't be read-only.
        private MudTabs _tabs;
#pragma warning restore IDE0044

        /// <summary>
        /// This field contains the authentication state.
        /// </summary>
        private AuthenticationState _authState;

        /// <summary>
        /// This field contains a reference to the form.
        /// </summary>
#pragma warning disable IDE0044  // can't be read-only.
        private MudForm _form;
#pragma warning restore IDE0044

        /// <summary>
        /// This field contains a list of in-progress uploads.
        /// </summary>
        private readonly IList<PreUpload> _preUploads = new List<PreUpload>();

        /// <summary>
        /// This fields contains a reference to breadcrumbs for the view.
        /// </summary>
        private readonly List<BreadcrumbItem> _crumbs = new()
        {
            new BreadcrumbItem("Home", href: "/"),
            new BreadcrumbItem("Uploads", href: "/uploads")
        };

        #endregion

        // *******************************************************************
        // Properties.
        // *******************************************************************

        #region Properties

        /// <summary>
        /// This property contains a reference to an application store.
        /// </summary>
        [Inject]
        private IApplicationStore ApplicationStore { get; set; }

        /// <summary>
        /// This property contains a reference to an environment store.
        /// </summary>
        [Inject]
        private IEnvironmentStore EnvironmentStore { get; set; }

        /// <summary>
        /// This property contains a reference to an upload store.
        /// </summary>
        [Inject]
        private IUploadStore UploadStore { get; set; }

        /// <summary>
        /// This property contains a reference to a settings store.
        /// </summary>
        [Inject]
        private ISettingStore SettingStore { get; set; }

        /// <summary>
        /// This property contains a reference to dialog service.
        /// </summary>
        [Inject]
        private IDialogService DialogService { get; set; }

        /// <summary>
        /// This property contains a reference to an authentication state provider.
        /// </summary>
        [Inject]
        private AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        #endregion

        // *******************************************************************
        // Protected methods.
        // *******************************************************************

        #region Protected methods

        /// <summary>
        /// This method invoked when the component is ready to start, having received 
        /// its initial parameters from its parent in the render tree. 
        /// </summary>
        /// <returns>A task to perform the operation.</returns>
        protected override async Task OnInitializedAsync()
        {
            // Give the base class a chance.
            await base.OnInitializedAsync();

            // Get the current authentication state.
            _authState = await AuthenticationStateProvider.GetAuthenticationStateAsync()
                .ConfigureAwait(false);
        }

        // *******************************************************************

        /// <summary>
        /// This method is called after Blazor renders the page.
        /// </summary>
        /// <param name="firstRender">True if this is the first render.</param>
        protected override void OnAfterRender(bool firstRender)
        {
            // Give the base class a chance.
            base.OnAfterRender(firstRender);

            // Only interested in first render.
            if (firstRender)
            {
                // Are there no existing uploads yet?
                if (false == SettingStore.AsQueryable().Any())
                {
                    // Show the new uploads tab.
#pragma warning disable BL0005
                    _tabs.ActivePanelIndex = 1;
#pragma warning restore
                }
            }
        }

        #endregion

        // *******************************************************************
        // Private methods.
        // *******************************************************************

        #region Private methods

        /// <summary>
        /// This method is called whenever a file is selected, by the user, for
        /// uploading.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        private void OnFileChange(
            InputFileChangeEventArgs e
            )
        {
            // Clear any previous state.
            _error = "";
            _info = "";

            // Loop through all the files that were chosen by the user.
            foreach (var file in e.GetMultipleFiles())
            {
                // Add each file as an in-progress upload.
                _preUploads.Add(new PreUpload() { File = file });
            }
        }

        // *******************************************************************

        /// <summary>
        /// This method is called whenever an application is selected for an
        /// upload.
        /// </summary>
        /// <param name="file">The file for the operation.</param>
        /// <param name="models">The application for the operation.</param>
        private void OnApplicationChanged(
            IBrowserFile file,
            HashSet<CG.Olive.Models.Application> models
            )
        {
            // Clear any previous state.
            _error = "";
            _info = "";

            // Look for an upload that corresponds to this file.
            var upload = _preUploads.FirstOrDefault(
                x => x.File == file
                );

            // Did we find one?
            if (null != upload)
            {
                // Set the application for the upload.
                upload.Application = models.FirstOrDefault();
            }
        }

        // *******************************************************************

        /// <summary>
        /// This method is called whenever an environment is selected for an
        /// upload.
        /// </summary>
        /// <param name="file">The file for the operation.</param>
        /// <param name="models">The environment for the operation.</param>
        private void OnEnvironmentChanged(
            IBrowserFile file,
            HashSet<CG.Olive.Models.Environment> models
            )
        {
            // Clear any previous state.
            _error = "";
            _info = "";

            // Look for an upload that corresponds to this file.
            var upload = _preUploads.FirstOrDefault(
                x => x.File == file
                );

            // Did we find one?
            if (null != upload)
            {
                // Set the environment for the upload.
                upload.Environment = models.FirstOrDefault();
            }
        }

        // *******************************************************************

        /// <summary>
        /// This method is called whenever the user presses the delete button
        /// for a pre-upload.
        /// </summary>
        /// <param name="model">The pre-upload for the operation.</param>
        private async Task OnDeletePreUploadAsync(PreUpload model)
        {
            // Clear any previous state.
            _error = "";
            _info = "";

            // Prompt the user first.
            bool? result = await DialogService.ShowMessageBox(
                "Warning",
                $"This will delete the upload for file: '{model.File.Name}'.",
                yesText: "OK!", 
                cancelText: "Cancel"
                ).ConfigureAwait(false);

            // Did the user press ok?
            if (result != null && result.Value)
            {
                // Remove the upload.
                _preUploads.Remove(model);
            }            
        }

        // *******************************************************************

        /// <summary>
        /// This method is called whenever the user presses the delete button
        /// for an existing upload.
        /// </summary>
        /// <param name="model">The upload for the operation.</param>
        private async Task OnDeleteUploadAsync(
            CG.Olive.Models.Upload model
            )
        {
            try
            {
                // Clear any previous state.
                _error = "";
                _info = "";

                // First, count the child settings (if any).
                var childCount = SettingStore.AsQueryable().Count(
                    x => x.UploadId == model.Id
                    );

                // Does this upload have child settings?
                if (0 < childCount)
                {
                    // If we get here then the upload has child settings so
                    //   we need to make the user jump through a hoop to avoid
                    //   unintended data loss.

                    // Create a model for the dialog.
                    var preDeleteModel = new PreDelete()
                    {
                        ModelName = model.FileName,
                        ChildCount = childCount
                    };

                    // Pass in the model.
                    var parameters = new DialogParameters
                    {
                        ["Model"] = preDeleteModel
                    };

                    // Fixup the dialog's look.
                    var options = new DialogOptions()
                    {
                        MaxWidth = MaxWidth.Small,
                        FullWidth = true
                    };

                    // Create the dialog.
                    var dialog = DialogService.Show<DeleteDialog>(
                        "",
                        parameters,
                        options
                        );

                    // Show the dialog.
                    var res = await dialog.Result.ConfigureAwait(false);

                    // Did the user cancel?
                    if (res.Cancelled)
                    {
                        return;
                    }
                }
                else
                {
                    // If we get here then the upload has no child settings.

                    // Prompt the user first.
                    bool? result = await DialogService.ShowMessageBox(
                        "Warning",
                        $"This will delete the upload for: '{model.FileName}'.",
                        yesText: "OK!",
                        cancelText: "Cancel"
                        ).ConfigureAwait(false);

                    // Did the user cancel?
                    if (result == null || false == result.Value)
                    {
                        return;
                    }
                }

                // If we get here then we've prompted the user, one way or 
                //   the other, and now we're ready to delete data.
                                
                // Defer to the store.
                await SettingStore.RollbackUploadAsync(
                    model,
                    _authState.User.GetEmail()
                    ).ConfigureAwait(false);
                
                // Tell the world what we did.
                if (0 < childCount)
                {
                    _info = $"Upload '{model.FileName}' and '{childCount}' child settings were deleted.";
                    
                }
                else
                {
                    _info = $"Upload '{model.FileName}' was deleted.";
                }

                // Update the UI.
                await InvokeAsync(() => StateHasChanged());
            }
            catch (Exception ex)
            {
                // Tell the user what went wrong.
                _error = ex.Message;
                _info = "";
            }
        }

        // *******************************************************************

        /// <summary>
        /// This method is called whenever the user presses the submit button
        /// for the form.
        /// </summary>
        private async Task OnSubmitAsync()
        {
            // Clear any previous state.
            _error = "";
            _info = "";

            // Validate the form.
            _form.Validate();

            // Is the form valid?
            if (_form.IsValid)
            {
                try
                {
                    var uploadCount = 0;
                    var settingCount = 0L;

                    // Loop through the uploads and process each one.
                    foreach (var model in _preUploads)
                    {
                        // Look for duplicates first.
                        Guard.Instance().ThrowIfDuplicate(model, UploadStore);

                        // Defer to the store.
                        var upload = await UploadStore.UploadFromJsonStreamAsync(
                            model.File.OpenReadStream(),
                            model.Application,
                            model.Environment,
                            model.File.Name,
                            _authState.User.GetEmail()
                            ).ConfigureAwait(false);

                        // Update the count.
                        uploadCount++;

                        // Defer to the store.
                        settingCount += await SettingStore.ApplyUploadAsync(
                            upload,
                            _authState.User.GetEmail()
                            ).ConfigureAwait(false);
                    }

                    // Tell the user what we did.
                    _info = $"{uploadCount} files were uploaded and {settingCount} settings were imported.";

                    // Done with the "in progress" uploads.
                    _preUploads.Clear();

                    // Switch to the existing uploads tab.
#pragma warning disable BL0005
                    _tabs.ActivePanelIndex = 0;
#pragma warning restore
                }
                catch (Exception ex)
                {
                    // Tell the user what went wrong.
                    _error = ex.Message;
                    _info = "";
                }
            }
        }

        // *******************************************************************

        /// <summary>
        /// This method is called whenever the user presses the properties button
        /// for a model.
        /// </summary>
        private async Task OnPropertiesAsync(
            Upload model
            )
        {
            // Clear any previous state.
            _error = "";
            _info = "";

            // Pass in the model.
            var parameters = new DialogParameters
            {
                ["Model"] = model
            };

            // Create the dialog.
            var dialog = DialogService.Show<AuditDialog<Upload>>(
                "",
                parameters
                );

            // Show the dialog.
            _ = await dialog.Result.ConfigureAwait(false);
        }

        #endregion
    }
}