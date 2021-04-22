using CG.Olive.Stores;
using CG.Olive.Web.Pages.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using Environment = CG.Olive.Models.Environment;

namespace CG.Olive.Web.Pages.Environments
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
        /// This field contains the authentication state.
        /// </summary>
        private AuthenticationState _authState;

        /// <summary>
        /// This fields contains a reference to breadcrumbs for the view.
        /// </summary>
        private readonly List<BreadcrumbItem> _crumbs = new()
        {
            new BreadcrumbItem("Home", href: "/"),
            new BreadcrumbItem("Environments", href: "/environments")
        };

        #endregion

        // *******************************************************************
        // Properties.
        // *******************************************************************

        #region Properties

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
            // Get the current authentication state.
            _authState = await AuthenticationStateProvider.GetAuthenticationStateAsync()
                .ConfigureAwait(false);

            // Give the base class a chance.s
            await base.OnInitializedAsync();
        }

        #endregion

        // *******************************************************************
        // Private methods.
        // *******************************************************************

        #region Private methods

        /// <summary>
        /// This method is called whenever the user presses the add button
        /// </summary>
        private async Task OnAddEnvironmentAsync()
        {
            try
            {
                // Reset any error / information.
                _error = "";
                _info = "";

                // Create a model.
                var model = new Environment()
                {
                    // First environment is automatically the default.
                    IsDefault = !EnvironmentStore.AsQueryable().Any(),

                    // Make the form validations happy.
                    CreatedBy = _authState.User.GetEmail(),
                    CreatedDate = DateTime.Now
                };

                // First environment is automatically called "Production"
                if (model.IsDefault)
                {
                    model.Name = "Production";
                }

                // Pass the model to the dialog.
                var parameters = new DialogParameters
                {
                    ["Model"] = model,
                    ["Caption"] = "Add Environment"
                };

                // Create the dialog.
                var dialog = DialogService.Show<EditDialog>(
                    "",
                    parameters
                    );

                // Show the dialog.
                var result = await dialog.Result.ConfigureAwait(false);

                // Did the user hit save?
                if (!result.Cancelled)
                {
                    // Defer to the store.
                    _ = await EnvironmentStore.AddAsync(
                        model
                        ).ConfigureAwait(false);

                    // Tell the world what we did.
                    _info = $"Environment: '{model.Name}' was created";
                }
            }
            catch (Exception ex)
            {
                // Save the error.
                _error = ex.Message;
            }

            // Update the UI.
            await InvokeAsync(() => StateHasChanged());
        }

        // *******************************************************************

        // *******************************************************************

        /// <summary>
        /// This method is called whenever the user presses the edit button
        /// for an existing environment.
        /// </summary>
        /// <param name="model">The model for the operation.</param>
        private async Task OnEditEnvironmentAsync(
            Environment model
            )
        {
            try
            {
                // Reset any error / information.
                _error = "";
                _info = "";

                // Clone the model.
                var temp = model.QuickClone();

                // Make the form validations happy.
                temp.UpdatedBy = _authState.User.GetEmail();
                temp.UpdatedDate = DateTime.Now;

                // Pass the clone to the dialog, so we won't have changed anything
                //   if the user eventually presses cancel.
                var parameters = new DialogParameters
                {
                    ["Model"] = temp,
                    ["Caption"] = $"Edit Environment: {temp.Name}"
                };

                // Create the dialog.
                var dialog = DialogService.Show<EditDialog>(
                    "Edit Environment",
                    parameters
                    );

                // Show the dialog.
                var result = await dialog.Result.ConfigureAwait(false);

                // Did the user hit save?
                if (!result.Cancelled)
                {
                    // Set the changes to the model.
                    model.Name = temp.Name;
                    model.UpdatedBy = temp.UpdatedBy;
                    model.UpdatedDate = temp.UpdatedDate;

                    // Defer to the store.
                    _ = await EnvironmentStore.UpdateAsync(
                        model
                        ).ConfigureAwait(false);

                    // Tell the world what we did.
                    _info = $"Environment: '{model.Name}' was updated";
                }
            }
            catch (Exception ex)
            {
                // Tell the world what happened.
                _error = ex.Message;
            }

            // Update the UI.
            await InvokeAsync(() => StateHasChanged());
        }

        // *******************************************************************

        /// <summary>
        /// This method is called whenever the user presses the delete button
        /// for an existing environment.
        /// </summary>
        /// <param name="model">The model for the operation.</param>
        private async Task OnDeleteEnvironmentAsync(
            Environment model
            )
        {
            try
            {
                // Reset any error.
                _error = "";
                _info = "";

                // First, check if we have data hanging off the environment.
                if (UploadStore.AsQueryable().Any(x => x.EnvironmentId == model.Id))
                {
                    // Prompt the user first.
                    await DialogService.ShowMessageBox(
                        "Problem",
                        $"The environment '{model.Name}' has one or more uploads associated with it " +
                        $"that MUST be manually removed before the environment can be deleted.",
                        yesText: "OK!"
                        );

                    // Nothing left to do.
                    return;
                }

                // Prompt the user first.
                bool? result = await DialogService.ShowMessageBox(
                    "Warning",
                    $"This will delete the environment: '{model.Name}'.",
                    yesText: "OK!",
                    cancelText: "Cancel"
                    );

                // Did the user press ok?
                if (result != null && result.Value)
                {
                    // Defer to the store.
                    await EnvironmentStore.DeleteAsync(
                        model.Id
                        ).ConfigureAwait(false);

                    // Tell the world what we did.
                    _info = $"Environment: '{model.Name}' was deleted";
                }
            }
            catch (Exception ex)
            {
                // Tell the world what happened.
                _error = ex.Message;
            }

            // Update the UI.
            await InvokeAsync(() => StateHasChanged());
        }

        // *******************************************************************

        /// <summary>
        /// This method is called whenever the user clicks the "Default" checkbox.
        /// </summary>
        /// <param name="model">The model to use for the operation.</param>
        /// <returns>A task to perform the operation.</returns>
        private async Task OnDefaultChangeAsync(Environment model)
        {
            try
            {
                // Reset any error / information.
                _error = "";
                _info = "";

                // Make sure we don't toggle the only environment.
                if (EnvironmentStore.AsQueryable().Count() <= 1)
                {
                    // Tell the world what the rule is.
                    _info = "Oops! First environment must be default!";
                    return;
                }

                // Setup the properties.
                model.UpdatedBy = _authState.User.GetEmail();
                model.UpdatedDate = DateTime.Now;

                // Set the changes to the model.
                model.IsDefault = !model.IsDefault;

                // Defer to the store.
                _ = await EnvironmentStore.UpdateAsync(
                    model
                    ).ConfigureAwait(false);

                // Tell the world what we did.
                _info = $"Environment: '{model.Name}' was {(model.IsDefault ? "made default" : "made not default")}";
            }
            catch (Exception ex)
            {
                // Tell the world what happened.
                _error = ex.Message;
            }

            // Update the UI.
            await InvokeAsync(() => StateHasChanged());
        }

        // *******************************************************************

        /// <summary>
        /// This method is called whenever the user presses the properties button
        /// for a model.
        /// </summary>
        private async Task OnPropertiesAsync(
            Environment model
            )
        {
            // Pass in the model.
            var parameters = new DialogParameters
            {
                ["Model"] = model
            };

            // Create the dialog.
            var dialog = DialogService.Show<AuditDialog<Environment>>(
                "",
                parameters
                );

            // Show the dialog.
            _ = await dialog.Result.ConfigureAwait(false);
        }

        #endregion
    }
}
