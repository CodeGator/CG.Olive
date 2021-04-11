using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

namespace CG.Olive.Web.Pages
{
    /// <summary>
    /// This class is the code-behind for the Error page.
    /// </summary>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [IgnoreAntiforgeryToken]
    public class ErrorModel : PageModel
    {
        // *******************************************************************
        // Properties.
        // *******************************************************************

        #region Properties

        /// <summary>
        /// This property contains a request identifier.
        /// </summary>
        public string RequestId { get; set; }

        /// <summary>
        /// This property indicates whether the request identifier is visible.
        /// </summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        #endregion

        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <summary>
        /// This method is called whenever the page is sent a GET verb.
        /// </summary>
        public void OnGet()
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        }

        #endregion
    }
}
