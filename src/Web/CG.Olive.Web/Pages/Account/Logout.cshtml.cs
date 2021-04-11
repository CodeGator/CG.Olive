using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

#pragma warning disable CS1998

namespace CG.Olive.Web.Pages.Account
{
    /// <summary>
    /// This class is the code-behind for the Logout page.
    /// </summary>
    public class LogoutModel : PageModel
    {
        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <summary>
        /// This method is called whenever the page is sent a POST verb.
        /// </summary>
        /// <returns>A task to perform the operation, that returns an <see cref="IActionResult"/>
        /// object.</returns>
        public async Task<IActionResult> OnPostAsync()
        {
            return SignOut(
                new AuthenticationProperties
                {
                    RedirectUri = "https://localhost:5001"
                },
                OpenIdConnectDefaults.AuthenticationScheme,
                CookieAuthenticationDefaults.AuthenticationScheme
                );
        }

        #endregion
    }
}