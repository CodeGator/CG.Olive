using CG.Linq.EFCore;
using CG.Olive.Repositories;
using CG.Olive.SqlServer;
using CG.Olive.SqlServer.Options;
using CG.Olive.SqlServer.Repositories;
using CG.Validations;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// This class contains extension methods related to the <see cref="IServiceCollection"/>
    /// type.
    /// </summary>
    /// <remarks>
    /// This class contains only those extension methods that are related to the logic
    /// within the <see cref="CG.Olive.SqlServer"/> library itself. 
    /// </remarks>
    public static partial class ServiceCollectionExtensions
    {
        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <summary>
        /// This method adds SQL Server repositories for the CG.Olive library.
        /// </summary>
        /// <param name="serviceCollection">The service collection to use for
        /// the operation.</param>
        /// <param name="configuration">The configuration to use for the operation.</param>
        /// <param name="serviceLifetime">The service lifetime to use for the operation.</param>
        /// <returns>The value of the <paramref name="serviceCollection"/> parameter,
        /// for chaining calls together.</returns>
        public static IServiceCollection AddSqlServerRepositories(
            this IServiceCollection serviceCollection,
            IConfiguration configuration,
            ServiceLifetime serviceLifetime = ServiceLifetime.Scoped
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(serviceCollection, nameof(serviceCollection))
                .ThrowIfNull(configuration, nameof(configuration));

            // Register the EFCORE options.
            serviceCollection.ConfigureOptions<OliveRepositoryOptions>(
                configuration
                );

            // Register the data-context.
            serviceCollection.AddTransient<OliveDbContext>(serviceProvider =>
            {
                // Get the options from the DI container.
                var options = serviceProvider.GetRequiredService<IOptions<OliveRepositoryOptions>>();

                // Create the options builder.
                var builder = new DbContextOptionsBuilder<OliveDbContext>();

                // Configure the options.
                builder.UseSqlServer(options.Value.ConnectionString);

                // Create the data-context.
                var context = new OliveDbContext(builder.Options);

                // Return the data-context.
                return context;
            });

            // Register the data-context factory.
            serviceCollection.Add<DbContextFactory<OliveDbContext>>(serviceLifetime);

            // Register the repositories.
            serviceCollection.Add<IApplicationRepository, ApplicationRepository>(serviceLifetime);
            serviceCollection.Add<IEnvironmentRepository, EnvironmentRepository>(serviceLifetime);
            serviceCollection.Add<IUploadRepository, UploadRepository>(serviceLifetime);
            serviceCollection.Add<ISettingRepository, SettingRepository>(serviceLifetime);

            // Return the service collection.
            return serviceCollection;
        }

        #endregion
    }
}
