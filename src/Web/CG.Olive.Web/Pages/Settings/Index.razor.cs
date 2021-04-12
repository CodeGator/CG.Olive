using CG.Olive.Models;
using CG.Olive.Stores;
using CG.Olive.Web.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using Environment = CG.Olive.Models.Environment;

namespace CG.Olive.Web.Pages.Settings
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
        /// This field contains the identifier of the selected application.
        /// </summary>
        private int _selectedApplicationId;

        /// <summary>
        /// This field contains the identifier of the selected environment.
        /// </summary>
        private int _selectedEnvironmentId;

        /// <summary>
        /// This field contains the settings query, for the grid.
        /// </summary>
        private IQueryable<Setting> _query = new Setting[0].AsQueryable();

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
        /// This property contains a reference to a setting store.
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

            QueryForSettings();
        }

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

            QueryForSettings();
        }

        /// <summary>
        /// This method queries for settings, using the currently selected
        /// application and environment.
        /// </summary>
        private void QueryForSettings()
        {
            if (_selectedEnvironmentId != 0 && _selectedApplicationId != 0)
            {
                _query = SettingStore.AsQueryable()
                    .Where(x => x.ApplicationId == _selectedApplicationId &&
                                x.EnvironmentId == _selectedEnvironmentId
                          ).OrderBy(x => x.Key);
            }
            else
            {
                _query = new Setting[0].AsQueryable();
            }

            StateHasChanged();
        }

        /// <summary>
        /// This method is called whenever the user presses the edit button
        /// for a setting.
        /// </summary>
        private async Task OnEditSettingAsync(
            Setting model
            )
        {
            try
            {
                // Reset any error / information.
                _error = "";
                _info = "";

                // Clone the model.
                var temp = model.QuickClone();

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
                    // Setup the properties.
                    model.UpdatedBy = _authState.User.GetEmail();
                    model.UpdatedDate = DateTime.Now;

                    // Set any change to the comment.
                    model.Comment = temp.Comment;

                    // Only change the value if it's not a parent node.
                    if (null != model.Value)
                    {
                        // Save any value change.
                        model.Value = temp.Value;
                    }

                    // Defer to the store.
                    _ = await SettingStore.UpdateAsync(
                        model
                        ).ConfigureAwait(false);

                    // Notify the back-channel.
                    await SignalRHub.OnChangeAsync(
                        model
                        ).ConfigureAwait(false);

                    // Tell the world what we did.
                    _info = $"Setting was updated";
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

        /// <summary>
        /// This method is called whenever the user presses the properties button
        /// for a setting.
        /// </summary>
        private async Task OnPropertiesAsync(
            Setting model
            )
        {
            // Pass in the model.
            var parameters = new DialogParameters
            {
                ["Model"] = model
            };

            // Create the dialog.
            var dialog = DialogService.Show<PropertiesDialog>(
                "",
                parameters
                );

            // Show the dialog.
            _ = await dialog.Result.ConfigureAwait(false);
        }

        #endregion
    }
}
