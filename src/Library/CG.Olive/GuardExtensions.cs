using CG.Olive.Stores;
using CG.Validations;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace CG.Olive.Models
{
    /// <summary>
    /// This class contains extension methods related to the <see cref="IGuard"/>
    /// type.
    /// </summary>
    /// <remarks>
    /// This class contains validation methods specific to the <see cref="CG.Olive"/>
    /// library.
    /// </remarks>
    public static partial class GuardExtensions
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
        /// <param name="argName">The argument name.</param>
        /// <param name="uploadStore">The upload store to use for the operation.</param>
        /// <param name="memberName">Not used. Supplied by the compiler.</param>
        /// <param name="sourceFilePath">Not used. Supplied by the compiler.</param>
        /// <param name="sourceLineNumber">Not used. Supplied by the compiler.</param>
        /// <returns>The <paramref name="guard"/> argument.</returns>
        /// <exception cref="ArgumentException">This exception is thrown when the <paramref name="argValue"/> 
        /// argument contains a potentially duplicate upload.</exception>
        public static IGuard ThrowIfDuplicate(
            this IGuard guard,
            Upload argValue,
            string argName,
            IUploadStore uploadStore,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0
            )
        {
            // Make the test.
            if (uploadStore.AsQueryable().Any(x => 
                x.Application == argValue.Application &&
                x.Environment == argValue.Environment 
                ))
            {
                // Panic!!!
                throw new ArgumentException(
                    message: $"There is already an upload for application " +
                        $"{argValue.Application.Name} and environment: " +
                        $"{argValue.Environment .Name}!\r\n[called from " +
                        $"{memberName} in {sourceFilePath}, line {sourceLineNumber}]",
                    paramName: argName
                    );
            }

            // Return the guard.
            return guard;
        }

        // *******************************************************************

        /// <summary>
        /// This method throws an exception if the <paramref name="argValue"/> 
        /// argument contains an environment that is not in an addable state.
        /// </summary>
        /// <param name="guard">The guard instance to use for the operation.</param>
        /// <param name="argValue">The argument to test.</param>
        /// <param name="argName">The argument name.</param>
        /// <param name="memberName">Not used. Supplied by the compiler.</param>
        /// <param name="sourceFilePath">Not used. Supplied by the compiler.</param>
        /// <param name="sourceLineNumber">Not used. Supplied by the compiler.</param>
        /// <returns>The <paramref name="guard"/> argument.</returns>
        /// <exception cref="ArgumentException">This exception is thrown when the <paramref name="argValue"/> 
        /// argument contains an environment that is not in an addable state.</exception>
        public static IGuard ThrowIfNotInAddableState(
            this IGuard guard,
            Environment argValue,
            string argName,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0
            )
        {
            // Make the test.
            if (argValue.Id != 0 || 
                string.IsNullOrEmpty(argValue.Name) ||
                string.IsNullOrEmpty(argValue.CreatedBy) ||
                argValue.UpdatedBy != null 
                )
            {
                // Panic!!!
                throw new ArgumentException(
                    message: $"The environment is not in an addable state! " +
                        $"[called from {memberName} in {sourceFilePath}, line {sourceLineNumber}]",
                    paramName: argName
                    );
            }

            // Return the guard.
            return guard;
        }

        // *******************************************************************

        /// <summary>
        /// This method throws an exception if the <paramref name="argValue"/> 
        /// argument contains an environment that is not in an updateable state.
        /// </summary>
        /// <param name="guard">The guard instance to use for the operation.</param>
        /// <param name="argValue">The argument to test.</param>
        /// <param name="argName">The argument name.</param>
        /// <param name="memberName">Not used. Supplied by the compiler.</param>
        /// <param name="sourceFilePath">Not used. Supplied by the compiler.</param>
        /// <param name="sourceLineNumber">Not used. Supplied by the compiler.</param>
        /// <returns>The <paramref name="guard"/> argument.</returns>
        /// <exception cref="ArgumentException">This exception is thrown when the <paramref name="argValue"/> 
        /// argument contains an environment that is not in an updateable state.</exception>
        public static IGuard ThrowIfNotInUpdateableState(
            this IGuard guard,
            Environment argValue,
            string argName,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0
            )
        {
            // Make the test.
            if (argValue.Id == 0 ||
                string.IsNullOrEmpty(argValue.Name) ||
                string.IsNullOrEmpty(argValue.CreatedBy) ||
                string.IsNullOrEmpty(argValue.UpdatedBy)
                )
            {
                // Panic!!!
                throw new ArgumentException(
                    message: $"The environment is not in an updateable state! " +
                        $"[called from {memberName} in {sourceFilePath}, line {sourceLineNumber}]",
                    paramName: argName
                    );
            }

            // Return the guard.
            return guard;
        }

        // *******************************************************************

        /// <summary>
        /// This method throws an exception if the <paramref name="argValue"/> 
        /// argument contains an application that is not in an addable state.
        /// </summary>
        /// <param name="guard">The guard instance to use for the operation.</param>
        /// <param name="argValue">The argument to test.</param>
        /// <param name="argName">The argument name.</param>
        /// <param name="memberName">Not used. Supplied by the compiler.</param>
        /// <param name="sourceFilePath">Not used. Supplied by the compiler.</param>
        /// <param name="sourceLineNumber">Not used. Supplied by the compiler.</param>
        /// <returns>The <paramref name="guard"/> argument.</returns>
        /// <exception cref="ArgumentException">This exception is thrown when the <paramref name="argValue"/> 
        /// argument contains an application that is not in an addable state.</exception>
        public static IGuard ThrowIfNotInAddableState(
            this IGuard guard,
            Application argValue,
            string argName,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0
            )
        {
            // Make the test.
            if (argValue.Id != 0 ||
                string.IsNullOrEmpty(argValue.Name) ||
                string.IsNullOrEmpty(argValue.Sid) ||
                string.IsNullOrEmpty(argValue.SKey) ||
                string.IsNullOrEmpty(argValue.CreatedBy) ||
                argValue.UpdatedBy != null
                )
            {
                // Panic!!!
                throw new ArgumentException(
                    message: $"The application is not in an addable state! " +
                        $"[called from {memberName} in {sourceFilePath}, line {sourceLineNumber}]",
                    paramName: argName
                    );
            }

            // Return the guard.
            return guard;
        }

        // *******************************************************************

        /// <summary>
        /// This method throws an exception if the <paramref name="argValue"/> 
        /// argument contains an environment that is not in an updateable state.
        /// </summary>
        /// <param name="guard">The guard instance to use for the operation.</param>
        /// <param name="argValue">The argument to test.</param>
        /// <param name="argName">The argument name.</param>
        /// <param name="memberName">Not used. Supplied by the compiler.</param>
        /// <param name="sourceFilePath">Not used. Supplied by the compiler.</param>
        /// <param name="sourceLineNumber">Not used. Supplied by the compiler.</param>
        /// <returns>The <paramref name="guard"/> argument.</returns>
        /// <exception cref="ArgumentException">This exception is thrown when the <paramref name="argValue"/> 
        /// argument contains an environment that is not in an updateable state.</exception>
        public static IGuard ThrowIfNotInUpdateableState(
            this IGuard guard,
            Application argValue,
            string argName,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0
            )
        {
            // Make the test.
            if (argValue.Id == 0 ||
                string.IsNullOrEmpty(argValue.Name) ||
                string.IsNullOrEmpty(argValue.CreatedBy) ||
                string.IsNullOrEmpty(argValue.UpdatedBy)
                )
            {
                // Panic!!!
                throw new ArgumentException(
                    message: $"The application is not in an updateable state! " +
                        $"[called from {memberName} in {sourceFilePath}, line {sourceLineNumber}]",
                    paramName: argName
                    );
            }

            // Return the guard.
            return guard;
        }

        #endregion
    }
}
