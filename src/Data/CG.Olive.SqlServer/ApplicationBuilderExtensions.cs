using CG.Olive.SqlServer;
using CG.Olive.SqlServer.Options;
using CG.Validations;
using System;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// This class contains extension methods related to the <see cref="IApplicationBuilder"/>
    /// type.
    /// </summary>
    /// <remarks>
    /// This class contains only those extension methods that are related to the logic
    /// within the <see cref="CG.Olive.SqlServer"/> library itself. 
    /// </remarks>
    public static partial class ApplicationBuilderExtensions
    {
        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <summary>
        /// This method wires up any startup logic required to support the 
        /// repositories and underyling SQL Server for the CG.Olive library.
        /// </summary>
        /// <param name="applicationBuilder">The application builder to use for
        /// the operation.</param>
        /// <param name="configurationSection">The configuration section name
        /// that corresponds with the repositories.</param>
        /// <returns>The value of the <paramref name="applicationBuilder"/> parameter,
        /// for chaining calls together.</returns>
        public static IApplicationBuilder UseSqlServerRepositories(
            this IApplicationBuilder applicationBuilder,
            string configurationSection
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(applicationBuilder, nameof(applicationBuilder))
                .ThrowIfNullOrEmpty(configurationSection, nameof(configurationSection));

            // Startup EFCore.
            applicationBuilder.UseEFCore<OliveDbContext, OliveRepositoryOptions>(
                (context, wasDropped, wasMigrated) =>
            {
                // Add seed data to the data-context.
                context.ApplySeedData();
            });

            // Return the application builder.
            return applicationBuilder;
        }

        #endregion
    }
}
