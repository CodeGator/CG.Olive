using CG.Olive.Models;
using CG.Olive.Stores;
using CG.Olive.Web.Pages.Shared;
using CG.Olive.Web.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Routing.Template;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using Environment = CG.Olive.Models.Environment;

namespace CG.Olive.Web.Pages.Features
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
        /// This field contains the selected application.
        /// </summary>
        private Application _selectedApplication;

        /// <summary>
        /// This field contains the selected environment.
        /// </summary>
        private Environment _selectedEnvironment;

        /// <summary>
        /// This field contains the feature query, for the grid.
        /// </summary>
        private IQueryable<Feature> _query = new Feature[0].AsQueryable();

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
            new BreadcrumbItem("Features", href: "/features")
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
        /// This property contains a reference to a feature store.
        /// </summary>
        [Inject]
        private IFeatureStore FeatureStore { get; set; }

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

        /// <summary>
        /// This property contains a reference to the back-channel SignalR hub.
        /// </summary>
        [Inject]
        private SignalRHub SignalRHub { get; set; }

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
        /// This method is called whenever the user selects an environment.
        /// </summary>
        /// <param name="models">The environments selected in the UI.</param>
        private void OnEnvironmentChanged(HashSet<Environment> models)
        {
            _selectedEnvironment = models.FirstOrDefault();

            QueryForFeatures();
        }

        // *******************************************************************

        /// <summary>
        /// This method is called whenever the user selects an application.
        /// </summary>
        /// <param name="models">The applications selected in the UI.</param>
        private void OnApplicationChanged(HashSet<Application> models)
        {
            _selectedApplication = models.FirstOrDefault();

            QueryForFeatures();
        }

        // *******************************************************************

        /// <summary>
        /// This method queries for features, using the currently selected
        /// application and environment.
        /// </summary>
        private void QueryForFeatures()
        {
            if (null != _selectedEnvironment && null != _selectedApplication)
            {
                _query = FeatureStore.AsQueryable()
                    .Where(x => x.ApplicationId == _selectedApplication.Id &&
                                x.EnvironmentId == _selectedEnvironment.Id
                          ).OrderBy(x => x.Key);
            }
            else
            {
                _query = new Feature[0].AsQueryable();
            }

            StateHasChanged();
        }

        // *******************************************************************

        /// <summary>
        /// This method is called whenever the user presses the edit button
        /// for a feature.
        /// </summary>
        private async Task OnEditFeatureAsync(
            Feature model
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
                    ["Caption"] = "Edit Feature"
                };

                // Create the dialog.
                var dialog = DialogService.Show<EditDialog>(
                    "Edit Feature",
                    parameters
                    );

                // Show the dialog.
                var result = await dialog.Result.ConfigureAwait(false);

                // Did the user hit save?
                if (!result.Cancelled)
                {
                    // Set the changes to the model.
                    model.Comment = temp.Comment;
                    model.Value = temp.Value;
                    model.UpdatedBy = temp.UpdatedBy;
                    model.UpdatedDate = temp.UpdatedDate;

                    // Defer to the store.
                    _ = await FeatureStore.UpdateAsync(
                        model
                        ).ConfigureAwait(false);

                    // Notify the back-channel.
                    await SignalRHub.OnChangeFeatureAsync(
                        model
                        ).ConfigureAwait(false);

                    // Tell the world what we did.
                    _info = $"Feature: '{model.Key}' was updated";
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
        /// This method is called whenever the user presses the properties button
        /// for a model.
        /// </summary>
        private async Task OnPropertiesAsync(
            Feature model
            )
        {
            // Pass in the model.
            var parameters = new DialogParameters
            {
                ["Model"] = model
            };

            // Create the dialog.
            var dialog = DialogService.Show<AuditDialog<Feature>>(
                "",
                parameters
                );

            // Show the dialog.
            _ = await dialog.Result.ConfigureAwait(false);
        }

        // *******************************************************************

        /// <summary>
        /// This method is called whenever the user presses the add button
        /// </summary>
        private async Task OnAddFeatureAsync()
        {
            try
            {
                // Reset any error / information.
                _error = "";
                _info = "";

                // Create a model.
                var model = new Feature()
                {
                    ApplicationId = _selectedApplication.Id,
                    EnvironmentId = _selectedEnvironment.Id,

                    // Make the form validations happy.
                    CreatedBy = _authState.User.GetEmail(),
                    CreatedDate = DateTime.Now
                };

                // Pass the model to the dialog.
                var parameters = new DialogParameters
                {
                    ["Model"] = model,
                    ["Caption"] = "Add Feature"
                };

                // Create the dialog.
                var dialog = DialogService.Show<EditDialog>(
                    "Add Feature",
                    parameters
                    );

                // Show the dialog.
                var result = await dialog.Result.ConfigureAwait(false);

                // Did the user hit save?
                if (!result.Cancelled)
                {
                    // Defer to the store.
                    _ = await FeatureStore.AddAsync(
                        model
                        ).ConfigureAwait(false);

                    // Tell the world what we did.
                    _info = $"Feature: '{model.Key}' was created";
                }
            }
            catch (Exception ex)
            {
                // Save the error.
                _error = ex.Message.Split('|').First();
            }

            // Update the UI.
            await InvokeAsync(() => StateHasChanged());
        }

        // *******************************************************************

        /// <summary>
        /// This method is called whenever the user presses the delete button
        /// for an existing feature.
        /// </summary>
        /// <param name="model">The model for the operation.</param>
        private async Task OnDeleteFeatureAsync(
            Feature model
            )
        {
            try
            {
                // Reset any error.
                _error = "";
                _info = "";

                // Prompt the user first.
                bool? result = await DialogService.ShowMessageBox(
                    "Warning",
                    $"This will delete the feature: '{model.Key}'.",
                    yesText: "OK!",
                    cancelText: "Cancel"
                    );

                // Did the user press ok?
                if (result != null && result.Value)
                {
                    // Defer to the store.
                    await FeatureStore.DeleteAsync(
                        model.Id
                        ).ConfigureAwait(false);

                    // Tell the world what we did.
                    _info = $"Feature: '{model.Key}' was deleted";
                }
            }
            catch (Exception ex)
            {
                // Tell the world what happened.
                _error = ex.Message.Split('|').First();
            }

            // Update the UI.
            await InvokeAsync(() => StateHasChanged());
        }

        #endregion
    }
}
