using CG.Olive.Models;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System.ComponentModel;
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
        public SignalRHub()
        {

        }

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
        public async Task OnChangeAsync(
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
                    "Change",
                    json,
                    cancellationToken
                    ).ConfigureAwait(false);
            }
        }

        #endregion
    }
}
