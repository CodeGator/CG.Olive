using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CG.Business.Managers;
using CG.Olive.Stores;
using CG.Validations;
using Microsoft.Extensions.Logging;

namespace CG.Olive.Managers
{
    /// <summary>
    /// This object is a default implementation of the <see cref="IFeatureSetManager"/>
    /// interface.
    /// </summary>
    public class FeatureSetManager : ManagerBase, IFeatureSetManager
    {
        // *******************************************************************
        // Properties.
        // *******************************************************************

        #region Properties

        /// <summary>
        /// This property contains a reference to a logger.
        /// </summary>
        protected ILogger<FeatureSetManager> Logger { get; }

        /// <summary>
        /// This property contains a reference to an application store.
        /// </summary>
        protected IApplicationStore ApplicationStore { get; }

        /// <summary>
        /// This property contains a reference to an environment store.
        /// </summary>
        protected IEnvironmentStore EnvironmentStore { get; }

        /// <summary>
        /// This property contains a reference to a feature store.
        /// </summary>
        protected IFeatureStore FeatureStore { get; }

        #endregion

        // *******************************************************************
        // Constructors.
        // *******************************************************************

        #region Constructors

        /// <summary>
        /// This constructor creates a new instance of the <see cref="FeatureSetManager"/>
        /// class.
        /// </summary>
        /// <param name="logger">The logger to use for the manager.</param>
        /// <param name="applicationStore">The application store to use with the manager.</param>
        /// <param name="environmentStore">The environment store to use with the manager.</param>
        /// <param name="featureStore">The feature store to use with the manager.</param>
        public FeatureSetManager(
            ILogger<FeatureSetManager> logger,
            IApplicationStore applicationStore,
            IEnvironmentStore environmentStore,
            IFeatureStore featureStore
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(logger, nameof(logger))
                .ThrowIfNull(applicationStore, nameof(applicationStore))
                .ThrowIfNull(environmentStore, nameof(environmentStore))
                .ThrowIfNull(featureStore, nameof(featureStore));

            // Save the refernces.
            Logger = logger;
            ApplicationStore = applicationStore;
            EnvironmentStore = environmentStore;
            FeatureStore = featureStore;
        }

        #endregion

        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <inheritdoc />
        public virtual Task<KeyValuePair<string, bool>[]> GetAsync(
            string sid,
            string skey,
            string environment,
            CancellationToken cancellationToken = default
            )
        {
            try
            {
                // Validate the parameters before attempting to use them.
                Guard.Instance().ThrowIfNull(sid, nameof(sid))
                    .ThrowIfNull(skey, nameof(skey));

                // First we find any settings for the 'default' environment, then
                //   we merge those with any settings for whatever environment was
                //   specified by the caller.

                // Look for a matching application.
                var appModel = ApplicationStore.AsQueryable().FirstOrDefault(
                    x => x.Sid == sid && x.SKey == skey && x.IsLocked == false
                    );

                // Did we fail to find one?
                if (null == appModel)
                {
                    // Panic!!
                    throw new ManagerException(
                        message: "Login failed!"
                        ).SetCallerInfo()
                         .SetOriginator(nameof(FeatureSetManager))
                         .SetDateTime();
                }

                // Look for the default environment.
                var defaultEnvModel = EnvironmentStore.AsQueryable().FirstOrDefault(
                    x => x.IsDefault == true
                    );

                // Was a default environment located?
                if (null == defaultEnvModel)
                {
                    // Panic!!
                    throw new ManagerException(
                        message: "No default enviroment located!"
                        ).SetCallerInfo()
                         .SetOriginator(nameof(FeatureSetManager))
                         .SetDateTime();
                }

                // Look for matching settings.
                var settings = FeatureStore.AsQueryable().Where(x =>
                    x.ApplicationId == appModel.Id &&
                    x.EnvironmentId == defaultEnvModel.Id
                    );

                var table = new Dictionary<string, bool>();

                // Loop and process settings.
                foreach (var setting in settings)
                {
                    // Use the setting's value.
                    table[setting.Key] = setting.Value;
                }

                // Was an environment specified by the caller?
                if (false == string.IsNullOrEmpty(environment))
                {
                    // Look for the specified environment.
                    var envModel = EnvironmentStore.AsQueryable().FirstOrDefault(
                        x => x.Name == environment
                        );

                    // Did we fail?
                    if (null == envModel)
                    {
                        // Panic!!
                        throw new ManagerException(
                            message: $"Environment '{environment}' not found!"
                            ).SetCallerInfo()
                             .SetOriginator(nameof(FeatureSetManager))
                             .SetDateTime();
                    }

                    // Is this the 'default' environment?
                    if (true == envModel.IsDefault)
                    {
                        // We're done.
                        return Task.FromResult(table.ToArray());
                    }

                    // If we get here then we've already loaded the 'default' environment's
                    //   settings, and now, we're about to merge those values with whatever
                    //   is in whatever environment was specified by the caller.

                    // Look for matching settings.
                    settings = FeatureStore.AsQueryable().Where(x =>
                        x.ApplicationId == appModel.Id &&
                        x.EnvironmentId == envModel.Id
                    );

                    // Loop and process settings.
                    foreach (var setting in settings)
                    {
                        // Use the setting's value.
                        table[setting.Key] = setting.Value;
                    }
                }

                // If we get here then we've built a collection of key-value-pair objects
                //   using the settings in our store. Nothing left to do but return that
                //   data to the caller.

                // Return the result.
                return Task.FromResult(table.ToArray());
            }
            catch (Exception ex)
            {
                // Provide better context for the error.
                throw new ManagerException(
                    message: $"Failed to query for a feature set!",
                    innerException: ex
                    ).SetCallerInfo()
                     .SetOriginator(nameof(FeatureSetManager))
                     .SetDateTime();
            }
        }

        #endregion
    }
}
