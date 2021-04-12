using CG.DataProtection;
using CG.Olive.Web.Options;
using CG.Olive.Web.Services;
using CG.Validations;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// This class contains extension methods related to the <see cref="IServiceCollection"/>
    /// type.
    /// </summary>
    /// <remarks>
    /// This class contains only those extension methods that are related to logic
    /// that lives within the <see cref="CG.Olive.Web"/> project. Look in other
    /// projects for extensions methods related to their logic - for instance,
    /// the extension methods for the <see cref="CG.Olive"/> project are located
    /// in the <see cref="CG.Olive"/> project.
    /// </remarks>
    public static partial class ServiceCollectionExtensions
    {
        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <summary>
        /// This method adds custom Blazor logic for the server.
        /// </summary>
        /// <param name="serviceCollection">The service collection to use for
        /// the operation.</param>
        /// <param name="configuration">The configuration to use for the operation.</param>
        /// <returns>The value of the <paramref name="serviceCollection"/> parameter,
        /// for chaining calls together.</returns>
        public static IServiceCollection AddCustomBlazor(
            this IServiceCollection serviceCollection,
            IConfiguration configuration
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(serviceCollection, nameof(serviceCollection))
                .ThrowIfNull(configuration, nameof(configuration));

            // Add support for razor pages.
            serviceCollection.AddRazorPages();

            // Add support for server-side Blazor.
            serviceCollection.AddServerSideBlazor();

            // Add support for controllers.
            serviceCollection.AddControllersWithViews();

            // Add the HTTP client.
            serviceCollection.AddHttpClient();

            // Return the service collection.
            return serviceCollection;
        }

        // *******************************************************************

        /// <summary>
        /// This method adds custom HTTP services for  the server.
        /// </summary>
        /// <param name="serviceCollection">The service collection to use for
        /// the operation.</param>
        /// <param name="configuration">The configuration to use for the operation.</param>
        /// <returns>The value of the <paramref name="serviceCollection"/> parameter,
        /// for chaining calls together.</returns>
        public static IServiceCollection AddCustomServices(
            this IServiceCollection serviceCollection,
            IConfiguration configuration
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(serviceCollection, nameof(serviceCollection))
                .ThrowIfNull(configuration, nameof(configuration));

            // Add a token provider service.
            serviceCollection.AddScoped<TokenProvider>();

            // Add a token service.
            serviceCollection.AddScoped<ITokenService, TokenService>();

            // Return the service collection.
            return serviceCollection;
        }

        // *******************************************************************

        /// <summary>
        /// This method adds custom identity logic for the server.
        /// </summary>
        /// <param name="serviceCollection">The service collection to use for
        /// the operation.</param>
        /// <param name="configuration">The configuration to use for the operation.</param>
        /// <returns>The value of the <paramref name="serviceCollection"/> parameter,
        /// for chaining calls together.</returns>
        public static IServiceCollection AddCustomIdentity(
            this IServiceCollection serviceCollection,
            IConfiguration configuration
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(serviceCollection, nameof(serviceCollection))
                .ThrowIfNull(configuration, nameof(configuration));

            // Configure the options.
            serviceCollection.ConfigureOptions(
                configuration.GetSection("Identity"),
                out IdentityOptions identityOptions
                );

            // Unprotect any protected properties.
            DataProtector.Instance().UnprotectProperties(
                identityOptions
                );

            // Add bearer token authentication for the internal API.
            serviceCollection.AddAuthentication("Bearer")
                .AddIdentityServerAuthentication("Bearer", options =>
                {
                    options.Authority = identityOptions.Authority;
                    options.ApiName = identityOptions.API.ApiName;                    
                    options.ApiSecret = identityOptions.API.ApiSecret;
                    options.SaveToken = true;
                });

            // Add an authentication policy for use with the internal API.
            var requireAuthenticatedUserPolicy = new AuthorizationPolicyBuilder()
                  .RequireAuthenticatedUser()
                  .Build();

            // Add cookie based openid authentication for the UI.
            serviceCollection.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            }).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
            {
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.Authority = identityOptions.Authority;
                options.ClientId = identityOptions.OIDC.ClientId;
                options.ClientSecret = identityOptions.OIDC.ClientSecret;
                options.ResponseType = "code";
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("email");
                options.Scope.Add("offline_access");
                options.Scope.Add("api1.read");
                options.Scope.Add("api1.write");
                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;
                options.TokenValidationParameters.NameClaimType = "given_name";
              });

            // Return the service collection.
            return serviceCollection;
        }

        // *******************************************************************

        /// <summary>
        /// This method adds services for the CG.Olive library.
        /// </summary>
        /// <param name="serviceCollection">The service collection to use for
        /// the operation.</param>
        /// <param name="configuration">The configuration to use for the operation.</param>
        /// <returns>The value of the <paramref name="serviceCollection"/> parameter,
        /// for chaining calls together.</returns>
        public static IServiceCollection AddCustomOlive(
            this IServiceCollection serviceCollection,
            IConfiguration configuration
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(serviceCollection, nameof(serviceCollection))
                .ThrowIfNull(configuration, nameof(configuration));

            // Add the custom Olive stores.
            serviceCollection.AddOliveStores(
                configuration.GetSection("CG.Olive")
                );

            // Add the custom Olive repositories.
            serviceCollection.AddRepositories(
                configuration.GetSection("CG.Olive:Repositories"),
                ServiceLifetime.Scoped
                );

            // Return the service collection.
            return serviceCollection;
        }

        // *******************************************************************

        /// <summary>
        /// This method adds custom SignalR services for the server.
        /// </summary>
        /// <param name="serviceCollection">The service collection to use for
        /// the operation.</param>
        /// <param name="configuration">The configuration to use for the operation.</param>
        /// <returns>The value of the <paramref name="serviceCollection"/> parameter,
        /// for chaining calls together.</returns>
        public static IServiceCollection AddCustomSignalR(
            this IServiceCollection serviceCollection,
            IConfiguration configuration
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(serviceCollection, nameof(serviceCollection))
                .ThrowIfNull(configuration, nameof(configuration));

            // Add a CORS policy.
            serviceCollection.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                    builder.SetIsOriginAllowed((host) => true)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

            // Add SignalR
            serviceCollection.AddSignalR();
            
            // Add our SignalR hub.
            serviceCollection.AddSingleton<SignalRHub>(x =>
            {
                // Create the scope.
                //var scope = x.CreateScope();

                // Create the hub.
                var hub = new SignalRHub();

                // Return the hub.
                return hub;
            });

            // Return the service collection.
            return serviceCollection;
        }

        #endregion
    }
}
