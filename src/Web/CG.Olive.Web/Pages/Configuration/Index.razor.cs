using CG.Olive.Models;
using CG.Olive.Stores;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Environment = CG.Olive.Models.Environment;

namespace CG.Olive.Web.Pages.Configuration
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
        /// This field indicates whether the page is busy, or not.
        /// </summary>
        public bool _isBusy;

        /// <summary>
        /// This field contains a summary error message.
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
        /// This field contains the configuration data.
        /// </summary>
        private IEnumerable<KeyValuePair<string, string>> _data = new KeyValuePair<string, string>[0].AsEnumerable();

        /// <summary>
        /// This fields contains a reference to breadcrumbs for the view.
        /// </summary>
        private readonly List<BreadcrumbItem> _crumbs = new()
        {
            new BreadcrumbItem("Home", href: "/"),
            new BreadcrumbItem("Configuration", href: "/configuration")
        };

        #endregion

        // *******************************************************************
        // Properties.
        // *******************************************************************

        #region Properties

        /// <summary>
        /// This property contains a reference to a configuration store.
        /// </summary>
        [Inject]
        private IConfigurationStore ConfigurationStore { get; set; }

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

        #endregion

        // *******************************************************************
        // Private methods.
        // *******************************************************************

        #region Private methods

        /// <summary>
        /// This method is called whenever the user selects an environment.
        /// </summary>
        /// <param name="models">The environments selected in the UI.</param>
        /// <returns>A task to perform the operation.</returns>
        private async Task OnEnvironmentChangedAsync(
            HashSet<Environment> models
            )
        {
            // Set the selected environment..
            _selectedEnvironment = models.FirstOrDefault();

            // Fetch data configuration.
            await QueryForDataAsync()
                .ConfigureAwait(false);
        }

        /// <summary>
        /// This method is called whenever the user selects an application.
        /// </summary>
        /// <param name="models">The applications selected in the UI.</param>
        /// <returns>A task to perform the operation.</returns>
        private async Task OnApplicationChangedAsync(
            HashSet<Application> models
            )
        {
            // Set the selected application.
            _selectedApplication = models.FirstOrDefault();

            // Fetch data configuration.
            await QueryForDataAsync()
                .ConfigureAwait(false);
        }

        /// <summary>
        /// This method queries for configuration key-value-pair objects, using the 
        /// currently selected application and environment.
        /// </summary>
        /// <returns>A task to perform the operation.</returns>
        private async Task QueryForDataAsync()
        {
            // Tell Blazor we are busy.
            _isBusy = true;

            try
            {
                // Clear the UI.
                _info = string.Empty;
                _error = string.Empty;

                // Are the application and environment selected?
                if (null != _selectedEnvironment && null != _selectedApplication)
                {
                    // Defer to the store.
                    _data = await ConfigurationStore.GetAsync(
                        _selectedApplication.Sid,
                        _selectedApplication.SKey,
                        _selectedEnvironment.Name
                        ).ConfigureAwait(false);
                }
                else
                {
                    // No data is possible.
                    _data = new KeyValuePair<string, string>[0].AsEnumerable();
                }
            }
            catch (Exception ex)
            {
                // Show the error.
                _error = ex.Message;
            }
            finally
            {
                // Tell Blazor we are no longer busy.
                _isBusy = false;
            }
        }

        #endregion
    }
}
