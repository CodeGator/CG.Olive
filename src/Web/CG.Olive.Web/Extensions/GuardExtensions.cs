using CG.Olive.Stores;
using CG.Olive.Web.Models;
using CG.Validations;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace CG.Olive.Web
{
    /// <summary>
    /// This class contains extension methods related to the <see cref="IGuard"/>
    /// type.
    /// </summary>
    /// <remarks>
    /// This class contains validation methods specific to the <see cref="CG.Olive.Web"/>
    /// application.
    /// </remarks>
    internal static partial class GuardExtensions
    {
        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <summary>
        /// This method throws an exception if the <paramref name="argValue"/> 
        /// argument contains an upload that, if processed, would create a duplicate.
        /// </summary>
        /// <param name="guard">The guard instance to use for the operation.</param>
        /// <param name="argValue">The argument to test.</param>
        /// <param name="uploadStore">The upload store to use for the operation.</param>
        /// <returns>The <paramref name="guard"/> argument.</returns>
        /// <exception cref="Exception">This exception is thrown when the <paramref name="argValue"/> 
        /// argument contains a potentially duplicate upload.</exception>
        public static IGuard ThrowIfDuplicate(
            this IGuard guard,
            PreUpload argValue,
            IUploadStore uploadStore
            )
        {
            // Make the test.
            if (uploadStore.AsQueryable().Any(x => 
                x.Application == argValue.Application &&
                x.Environment == argValue.Environment 
                ))
            {
                // Panic!!!
                throw new Exception(
                    message: $"There is already an upload for application " +
                        $"{argValue.Application.Name} and environment: " +
                        $"{argValue.Environment .Name}!"
                    );
            }

            // Return the guard.
            return guard;
        }

        #endregion
    }
}
