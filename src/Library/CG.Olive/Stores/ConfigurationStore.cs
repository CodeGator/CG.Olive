using CG.Business.Stores;
using CG.Secrets.Stores;
using CG.Validations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CG.Olive.Stores
{
    /// <summary>
    /// This class is a default implementation of the <see cref="IConfigurationStore"/>
    /// interface.
    /// </summary>
    public class ConfigurationStore : StoreBase, IConfigurationStore
    {
        // *******************************************************************
        // Properties.
        // *******************************************************************

        #region Properties

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
        /// This constructor creates a new instance of the <see cref="ConfigurationStore"/>
        /// class.
        /// </summary>
        /// <param name="applicationStore">The application store to use with the store.</param>
        /// <param name="environmentStore">The environment store to use with the store.</param>
        /// <param name="settingStore">The setting store to use with the store.</param>
        /// <param name="secretStore">The secret store to use with the store.</param>
        public ConfigurationStore(
            IApplicationStore applicationStore,
            IEnvironmentStore environmentStore,
            ISettingStore settingStore,
            ISecretStore secretStore
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(applicationStore, nameof(applicationStore))
                .ThrowIfNull(environmentStore, nameof(environmentStore))
                .ThrowIfNull(secretStore, nameof(secretStore))
                .ThrowIfNull(settingStore, nameof(settingStore));

            // Save the refernces.
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
        public virtual Task<KeyValuePair<string, string>[]> GetAsync(
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

                // Look for a matching application.
                var appModel = ApplicationStore.AsQueryable().FirstOrDefault(
                    x => x.Sid == sid && x.SKey == skey && x.IsLocked == false
                    );

                // Did we fail to find one?
                if (null == appModel)
                {
                    // Panic!!
                    throw new StoreException(
                        message: "Login failed!"
                        ).SetCallerInfo()
                         .SetOriginator(nameof(ConfigurationStore))
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
                    throw new StoreException(
                        message: "No default enviroment located!"
                        ).SetCallerInfo()
                         .SetOriginator(nameof(ConfigurationStore))
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
                    // Add the setting to the table.
                    table[setting.Key] = setting.Value;
                }

                // Was an environment specified?
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
                        throw new StoreException(
                            message: $"Environment '{environment}' not found!"
                            ).SetCallerInfo()
                             .SetOriginator(nameof(ConfigurationStore))
                             .SetDateTime();
                    }

                    // Look for matching settings.
                    settings = SettingStore.AsQueryable().Where(x =>
                        x.ApplicationId == appModel.Id &&
                        x.EnvironmentId == envModel.Id
                    );

                    // Loop and process settings.
                    foreach (var setting in settings)
                    {
                        // Add the setting to the table.
                        table[setting.Key] = setting.Value;
                    }
                }

                //var foo = SecretStore.GetByNameAsync("Test1").Result;

                // Return the result.
                return Task.FromResult(
                    table.ToArray()
                    );
            }
            catch (Exception ex)
            {
                // Provide better context for the error.
                throw new StoreException(
                    message: $"Failed to query for a configuration!",
                    innerException: ex
                    ).SetCallerInfo()
                     .SetOriginator(nameof(ConfigurationStore))
                     .SetDateTime();
            }
        }

        #endregion
    }
}

