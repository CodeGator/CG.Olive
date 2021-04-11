using CG.Validations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// This class contains extension methods related to the <see cref="IApplicationBuilder"/>
    /// type.
    /// </summary>
    public static partial class ApplicationBuilderExtensions
    {
        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <summary>
        /// This method wires up any startup logic required, by Blazor, to support
        /// the server.
        /// </summary>
        /// <param name="applicationBuilder">The application builder to use for
        /// the operation.</param>
        /// <param name="webHostEnvironment">The web host environment to use 
        /// for the operation.</param>
        /// <returns>The value of the <paramref name="applicationBuilder"/> parameter,
        /// for chaining calls together.</returns>
        public static IApplicationBuilder UseCustomBlazor(
            this IApplicationBuilder applicationBuilder,
            IWebHostEnvironment webHostEnvironment
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(applicationBuilder, nameof(applicationBuilder))
                .ThrowIfNull(webHostEnvironment, nameof(webHostEnvironment));

            // Are we running in a development environment?
            if (webHostEnvironment.IsDevelopment())
            {
                // Use the developer error page.
                applicationBuilder.UseDeveloperExceptionPage();
            }
            else
            {
                // Use the standard error handler.
                applicationBuilder.UseExceptionHandler("/Error");

                // Use HSTS
                applicationBuilder.UseHsts();
            }

            // Use HTTPS
            applicationBuilder.UseHttpsRedirection();

            // Use static files.
            applicationBuilder.UseStaticFiles();

            // Use routing.
            applicationBuilder.UseRouting();

            // Use authentication.
            applicationBuilder.UseAuthentication();

            // Use authorization.
            applicationBuilder.UseAuthorization();

            // Use endpoint routing.
            applicationBuilder.UseEndpoints(endpoints =>
            {
                // Map the Blazor hub.
                endpoints.MapBlazorHub();

                // Map the fallback page.
                endpoints.MapFallbackToPage("/_Host");

                // Map the default controller route.
                endpoints.MapDefaultControllerRoute();
            });

            // Use the Olive epositories.
            applicationBuilder.UseRepositories(
                "CG.Olive:Repositories"
                );

            // Return the application builder.
            return applicationBuilder;
        }

        #endregion
    }
}
