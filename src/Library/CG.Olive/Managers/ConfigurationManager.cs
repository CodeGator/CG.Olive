using CG.Business.Managers;
using CG.Business.Stores;
using CG.Olive.Stores;
using CG.Secrets.Stores;
using CG.Validations;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CG.Olive.Managers
{
    /// <summary>
    /// This class is a default implementation of the <see cref="IConfigurationManager"/>
    /// interface.
    /// </summary>
    public class ConfigurationManager : ManagerBase, IConfigurationManager
    {
        // *******************************************************************
        // Properties.
        // *******************************************************************

        #region Properties

        /// <summary>
        /// This property contains a reference to a logger.
        /// </summary>
        protected ILogger<ConfigurationManager> Logger { get; }

        /// <summary>
        /// This property contains a reference to an application store.
        /// </summary>
        protected IApplicationStore ApplicationStore { get; }

        /// <summary>
        /// This property contains a reference to an environment store.
        /// </summary>
        protected IEnvironmentStore EnvironmentStore { get; }

        /// <summary>
        /// This property contains a reference to a setting store.
        /// </summary>
        protected ISettingStore SettingStore { get; }

        /// <summary>
        /// This property contains a reference to a secret store.
        /// </summary>
        protected ISecretStore SecretStore { get; }

        #endregion

        // *******************************************************************
        // Constructors.
        // *******************************************************************

        #region Constructors

        /// <summary>
        /// This constructor creates a new instance of the <see cref="ConfigurationManager"/>
        /// class.
        /// </summary>
        /// <param name="logger">The logger to use for the manager.</param>
        /// <param name="applicationStore">The application store to use with the manager.</param>
        /// <param name="environmentStore">The environment store to use with the manager.</param>
        /// <param name="settingStore">The setting store to use with the manager.</param>
        /// <param name="secretStore">The secret store to use with the manager.</param>
        public ConfigurationManager(
            ILogger<ConfigurationManager> logger,
            IApplicationStore applicationStore,
            IEnvironmentStore environmentStore,
            ISettingStore settingStore,
            ISecretStore secretStore
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(logger, nameof(logger))
                .ThrowIfNull(applicationStore, nameof(applicationStore))
                .ThrowIfNull(environmentStore, nameof(environmentStore))
                .ThrowIfNull(secretStore, nameof(secretStore))
                .ThrowIfNull(settingStore, nameof(settingStore));

            // Save the refernces.
            Logger = logger;
            ApplicationStore = applicationStore;
            EnvironmentStore = environmentStore;
            SettingStore = settingStore;
            SecretStore = secretStore;
        }

        #endregion

        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <inheritdoc />
        public virtual async Task<KeyValuePair<string, string>[]> GetAsync(
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
                         .SetOriginator(nameof(ConfigurationManager))
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
                         .SetOriginator(nameof(ConfigurationManager))
                         .SetDateTime();
                }

                // Look for matching settings.
                var settings = SettingStore.AsQueryable().Where(x =>
                    x.ApplicationId == appModel.Id &&
                    x.EnvironmentId == defaultEnvModel.Id
                    );

                var table = new Dictionary<string, string>();

                // Loop and process settings.
                foreach (var setting in settings)
                {
                    // Is the setting a secret?
                    if (setting.IsSecret)
                    {
                        // Is the value, which is the secret key, missing?
                        if (string.IsNullOrEmpty(setting.Value))
                        {
                            // Let someone know what happened.
                            Logger.LogWarning(
                                $"The settting '{setting.Key}', in environment: '{defaultEnvModel.Name}', " +
                                $"for sid: '{sid}', is marked as a secret, but, there is no value in the setting! " +
                                $"reverting to value: '{setting.Value}'"
                                );

                            // Fallback to using the setting's value.
                            table[setting.Key] = setting.Value;
                        }
                        else
                        {
                            // If we get here, we need to fetch the actual secret from the remote
                            //   secret store, and put that value into the key-value-pair.

                            try
                            {
                                // Try to fetch the specified secret.
                                var secret = await SecretStore.GetByNameAsync(
                                    setting.Value,
                                    cancellationToken
                                    ).ConfigureAwait(false);

                                // Use the secret value.
                                table[setting.Key] = secret.Value;
                            }
                            catch (Exception ex)
                            {
                                // Let someone know what happened.
                                Logger.LogWarning(
                                    $"Failed to find a secret for setting: '{setting.Key}', " +
                                    $"in environment: '{defaultEnvModel.Name}', " +
                                    $"for sid: '{sid}', " +
                                    $"reverting to value: '{setting.Value}'",
                                    ex
                                    );

                                // Fallback to using the setting's value.
                                table[setting.Key] = setting.Value;
                            }
                        }
                    }
                    else
                    {
                        // Use the setting's value.
                        table[setting.Key] = setting.Value;
                    }
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
                             .SetOriginator(nameof(ConfigurationManager))
                             .SetDateTime();
                    }

                    // Is this the 'default' environment?
                    if (true == envModel.IsDefault)
                    {
                        // We're done.
                        return table.ToArray(); 
                    }

                    // If we get here then we've already loaded the 'default' environment's
                    //   settings, and now, we're about to merge those values with whatever
                    //   is in whatever environment was specified by the caller.

                    // Look for matching settings.
                    settings = SettingStore.AsQueryable().Where(x =>
                        x.ApplicationId == appModel.Id &&
                        x.EnvironmentId == envModel.Id
                    );

                    // Loop and process settings.
                    foreach (var setting in settings)
                    {
                        // Is the setting a secret?
                        if (setting.IsSecret)
                        {
                            // Is the value, which is the secret key, missing?
                            if (string.IsNullOrEmpty(setting.Value))
                            {
                                // Let someone know what happened.
                                Logger.LogWarning(
                                    $"The settting '{setting.Key}', in environment: '{environment}', " +
                                    $"for sid: '{sid}', is marked as a secret, but, there is no value in the setting! " +
                                    $"reverting to value: '{setting.Value}'"
                                    );

                                // Fallback to using the setting's value.
                                table[setting.Key] = setting.Value;
                            }
                            else
                            {
                                // If we get here, we need to fetch the actual secret from the remote
                                //   secret store, and put that value into the key-value-pair.

                                try
                                {
                                    // Try to fetch the specified secret.
                                    var secret = await SecretStore.GetByNameAsync(
                                        setting.Value,
                                        cancellationToken
                                        ).ConfigureAwait(false);

                                    // Use the secret value.
                                    table[setting.Key] = secret.Value;
                                }
                                catch (Exception ex)
                                {
                                    // Let the world know what happened.
                                    Logger.LogWarning(
                                        $"Failed to find a secret for setting: '{setting.Key}', " +
                                        $"in environment: '{environment}', " +
                                        $"for sid: '{sid}', " +
                                        $"reverting to value: '{setting.Value}'",
                                        ex
                                        );

                                    // Fallback to using the setting's value.
                                    table[setting.Key] = setting.Value;
                                }
                            }
                        }
                        else
                        {
                            // Use the setting's value.
                            table[setting.Key] = setting.Value;
                        }
                    }
                }

                // If we get here then we've built a collection of key-value-pair objects
                //   using the settings in our store. Nothing left to do but return that
                //   data to the caller.

                // Return the result.
                return table.ToArray();
            }
            catch (Exception ex)
            {
                // Provide better context for the error.
                throw new ManagerException(
                    message: $"Failed to query for a configuration!",
                    innerException: ex
                    ).SetCallerInfo()
                     .SetOriginator(nameof(ConfigurationManager))
                     .SetDateTime();
            }
        }

        #endregion
    }
}

