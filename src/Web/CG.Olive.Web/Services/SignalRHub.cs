using CG.Olive.Models;
using Microsoft.AspNetCore.SignalR;
using System.Threading;
using System.Threading.Tasks;

namespace CG.Olive.Web.Services
{
    /// <summary>
    /// This class is a SignalR hub for the client back channel.
    /// </summary>
    public class SignalRHub : Hub
    {
        // *******************************************************************
        // Constructors.
        // *******************************************************************

        #region Constructors

        /// <summary>
        /// This constructor creates a new instance of the <see cref="SignalRHub"/>
        /// class.
        /// </summary>
        public SignalRHub() { }

        #endregion

        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <summary>
        /// This method sends a change notification.
        /// </summary>
        /// <param name="model">The setting that was changed.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A task to perform the operation.</returns>
        public async Task OnChangeSettingAsync(
            Setting model,
            CancellationToken cancellationToken = default
            )
        {
            // Do we have any clients to notify?
            if (Clients != null && Clients.All != null)
            {
                // Serialize the model.
                var json = "{" +
                    "\"Key\": \"" + model.Key + "\"," +
                    "\"Sid\": \"" + model.Application.Sid + "\"," +
                    "}";

                // Send the message to everyone.
                await Clients.All.SendAsync(
                    "ChangeSetting",
                    json,
                    cancellationToken
                    ).ConfigureAwait(false);
            }
        }

        // *******************************************************************

        /// <summary>
        /// This method sends a change notification.
        /// </summary>
        /// <param name="model">The setting that was changed.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A task to perform the operation.</returns>
        public async Task OnChangeFeatureAsync(
            Feature model,
            CancellationToken cancellationToken = default
            )
        {
            // Do we have any clients to notify?
            if (Clients != null && Clients.All != null)
            {
                // Serialize the model.
                var json = "{" +
                    "\"Key\": \"" + model.Key + "\"," +
                    "\"Sid\": \"" + model.Application.Sid + "\"," +
                    "}";

                // Send the message to everyone.
                await Clients.All.SendAsync(
                    "ChangeFeature",
                    json,
                    cancellationToken
                    ).ConfigureAwait(false);
            }
        }

        #endregion
    }
}
