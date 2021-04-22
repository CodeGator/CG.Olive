using CG.Olive.Managers;
using CG.Olive.Stores;
using CG.Validations;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// This class contains extension methods related to the <see cref="IServiceCollection"/>
    /// type.
    /// </summary>
    /// <remarks>
    /// This class contains only those extension methods that are related to logic
    /// that lives within the <see cref="CG.Olive"/> project. Look in other projects 
    /// for extensions methods related to their logic - for instance, the extension 
    /// methods for the CG.Olive.Web project are located in the CG.Olive.Web project, 
    /// and the extension methods for the CG.Olive.SqlServer project are located in 
    /// the CG.Olive.SqlServer project.
    /// </remarks>
    public static partial class ServiceCollectionExtensions
    {
        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <summary>
        /// This method adds stores for the Olive library.
        /// </summary>
        /// <param name="serviceCollection">The service collection to use for
        /// the operation.</param>
        /// <param name="configuration">The configuration to use for the operation.</param>
        /// <param name="serviceLifetime">The service lifetime to use for the operation.</param>
        /// <returns>The value of the <paramref name="serviceCollection"/> parameter,
        /// for chaining calls together.</returns>
        public static IServiceCollection AddOliveStores(
            this IServiceCollection serviceCollection,
            IConfiguration configuration,
            ServiceLifetime serviceLifetime = ServiceLifetime.Scoped
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(serviceCollection, nameof(serviceCollection))
                .ThrowIfNull(configuration, nameof(configuration));

            // Register the stores.
            serviceCollection.Add<IApplicationStore, ApplicationStore>(serviceLifetime);
            serviceCollection.Add<IEnvironmentStore, EnvironmentStore>(serviceLifetime);
            serviceCollection.Add<IUploadStore, UploadStore>(serviceLifetime);
            serviceCollection.Add<ISettingStore, SettingStore>(serviceLifetime);
            serviceCollection.Add<IFeatureStore, FeatureStore>(serviceLifetime);

            // Return the service collection.
            return serviceCollection;
        }

        // *******************************************************************

        /// <summary>
        /// This method adds managers for the Olive library.
        /// </summary>
        /// <param name="serviceCollection">The service collection to use for
        /// the operation.</param>
        /// <param name="configuration">The configuration to use for the operation.</param>
        /// <param name="serviceLifetime">The service lifetime to use for the operation.</param>
        /// <returns>The value of the <paramref name="serviceCollection"/> parameter,
        /// for chaining calls together.</returns>
        public static IServiceCollection AddOliveManagers(
            this IServiceCollection serviceCollection,
            IConfiguration configuration,
            ServiceLifetime serviceLifetime = ServiceLifetime.Scoped
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(serviceCollection, nameof(serviceCollection))
                .ThrowIfNull(configuration, nameof(configuration));

            // Register the managers.
            serviceCollection.Add<IFeatureSetManager, FeatureSetManager>(serviceLifetime);
            serviceCollection.Add<IConfigurationManager, ConfigurationManager>(serviceLifetime);

            // Return the service collection.
            return serviceCollection;
        }

        #endregion
    }
}
