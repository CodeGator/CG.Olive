using CG.Olive.Stores;
using CG.Validations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CG.Olive.Web.Controllers
{
    /// <summary>
    /// This class is a REST controller for configuration related operations.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigurationController : ControllerBase
    {
        // *******************************************************************
        // Fields.
        // *******************************************************************

        #region Fields

        /// <summary>
        /// This field contains a reference to a configuration store.
        /// </summary>
        private readonly IConfigurationStore _configurationStore;

        #endregion

        // *******************************************************************
        // Constructors.
        // *******************************************************************

        #region Constructors

        /// <summary>
        /// This constructor creates a new instance of the <see cref="ConfigurationController"/>
        /// class.
        /// </summary>

        /// <param name="configurationStore">The configuration store to use with the controller.</param>
        public ConfigurationController(
            IConfigurationStore configurationStore
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(configurationStore, nameof(configurationStore));

            // Save the references.
            _configurationStore = configurationStore;
        }

        #endregion

        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <summary>
        /// This method retrieves a configuration for the specified application
        /// and environment.
        /// </summary>
        /// <param name="sid">The application security identifier to use for the operation.</param>
        /// <param name="skey">The application security key to use for the operation.</param>
        /// <param name="environment">The optional environment name for the operation.</param>
        /// <returns>A task to perform the operation that returns a <see cref="IActionResult"/>
        /// instance, with the results of the operation.</returns>
        [AllowAnonymous]
        [HttpGet("{sid}/{skey}/{environment?}")]
        public virtual async Task<IActionResult> GetAsync(
            string sid,
            string skey,
            string environment
            )
        {
            try
            {
                // Validate the parameters before attempting to use them.
                Guard.Instance().ThrowIfNullOrEmpty(sid, nameof(sid))
                    .ThrowIfNullOrEmpty(skey, nameof(skey));

                // Defer to the store.
                var settings = await _configurationStore.GetAsync(
                    sid,
                    skey, 
                    environment
                    ).ConfigureAwait(false);

                // Return the results.
                return Ok(settings);
            }
            catch (Exception ex)
            {
                // TODO : raise an error alert.

                // Return a summary of the problem.
                return Problem(
                    title: $"{nameof(ConfigurationController)} error!",
                    detail: $"Message: {ex.Message}",
                    statusCode: StatusCodes.Status500InternalServerError
                    );
            }            
        }

        #endregion
    }
}
