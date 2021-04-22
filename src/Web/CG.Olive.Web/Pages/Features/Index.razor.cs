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
        /// This field contains the identifier of the selected application.
        /// </summary>
        private int _selectedApplicationId;

        /// <summary>
        /// This field contains the identifier of the selected environment.
        /// </summary>
        private int _selectedEnvironmentId;

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
            var environment = models.FirstOrDefault();
            if (null != environment)
            {
                _selectedEnvironmentId = environment.Id;
            }
            else
            {
                _selectedEnvironmentId = 0;
            }

            QueryForFeatures();
        }

        // *******************************************************************

        /// <summary>
        /// This method is called whenever the user selects an application.
        /// </summary>
        /// <param name="models">The applications selected in the UI.</param>
        private void OnApplicationChanged(HashSet<Application> models)
        {
            var application = models.FirstOrDefault();
            if (null != application)
            {
                _selectedApplicationId = application.Id;
            }
            else
            {
                _selectedApplicationId = 0;
            }

            QueryForFeatures();
        }

        // *******************************************************************

        /// <summary>
        /// This method queries for features, using the currently selected
        /// application and environment.
        /// </summary>
        private void QueryForFeatures()
        {
            if (_selectedEnvironmentId != 0 && _selectedApplicationId != 0)
            {
                _query = FeatureStore.AsQueryable()
                    .Where(x => x.ApplicationId == _selectedApplicationId &&
                                x.EnvironmentId == _selectedEnvironmentId
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
                    ["Model"] = temp
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
                    _info = $"Feature was updated";
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

        #endregion
    }
}
