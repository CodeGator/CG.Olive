using CG.Business.Stores;
using CG.Olive.Models;
using CG.Olive.Repositories;
using CG.Validations;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CG.Olive.Stores
{
    /// <summary>
    /// This class is a default implementation of the <see cref="IApplicationStore"/>
    /// interface.
    /// </summary>
    public class ApplicationStore : StoreBase, IApplicationStore
    {
        // *******************************************************************
        // Properties.
        // *******************************************************************

        #region Properties

        /// <summary>
        /// This property contains a reference to a logger.
        /// </summary>
        protected ILogger<ApplicationStore> Logger { get; }

        /// <summary>
        /// This property contains a reference to an application repository.
        /// </summary>
        protected IApplicationRepository ApplicationRepository { get; }

        #endregion

        // *******************************************************************
        // Constructors.
        // *******************************************************************

        #region Constructors

        /// <summary>
        /// This constructor creates a new instance of the <see cref="ApplicationStore"/>
        /// class.
        /// </summary>
        /// <param name="logger">The logger to use for the store.</param>
        /// <param name="applicationRepository">The application repository to use
        /// with the store.</param>
        public ApplicationStore(
            ILogger<ApplicationStore> logger,
            IApplicationRepository applicationRepository
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(logger, nameof(logger))
                .ThrowIfNull(applicationRepository, nameof(applicationRepository));

            // Save the references.
            Logger = logger;
            ApplicationRepository = applicationRepository;
        }

        #endregion

        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <inheritdoc />
        public virtual IQueryable<Application> AsQueryable()
        {
            try
            {
                // Defer to the repository.
                var query = ApplicationRepository.AsQueryable();

                // Return the results.
                return query;
            }
            catch (Exception ex)
            {
                // Provide better context for the error.
                throw new StoreException(
                    message: $"Failed to query for applications!",
                    innerException: ex
                    ).SetCallerInfo()
                     .SetOriginator(nameof(ApplicationStore))
                     .SetDateTime();
            }
        }

        // *******************************************************************

        /// <inheritdoc />
        public virtual async Task<Application> AddAsync(
            Application model,
            CancellationToken cancellationToken = default
            )
        {
            try
            {
                // Validate the parameters before attempting to use them.
                Guard.Instance().ThrowIfNull(model, nameof(model))
                    .ThrowIfNotInAddableState(model, nameof(model));

                // Defer to the repository.
                var addedModel = await ApplicationRepository.AddAsync(
                    model,
                    cancellationToken
                    ).ConfigureAwait(false);

                // Return the newly added model.
                return addedModel;
            }
            catch (Exception ex)
            {
                // Provide better context for the error.
                throw new StoreException(
                    message: $"Failed to add a new application!",
                    innerException: ex
                    ).SetCallerInfo()
                     .SetOriginator(nameof(ApplicationStore))
                     .SetMethodArguments(("model", model))
                     .SetDateTime();
            }
        }

        // *******************************************************************

        /// <inheritdoc />
        public virtual async Task<Application> UpdateAsync(
            Application model,
            CancellationToken cancellationToken = default
            )
        {
            try
            {
                // Validate the parameters before attempting to use them.
                Guard.Instance().ThrowIfNull(model, nameof(model))
                    .ThrowIfNotInUpdateableState(model, nameof(model));

                // Defer to the repository.
                var updatedModel = await ApplicationRepository.UpdateAsync(
                    model,
                    cancellationToken
                    ).ConfigureAwait(false);

                // Return the newly updated model.
                return updatedModel;
            }
            catch (Exception ex)
            {
                // Provide better context for the error.
                throw new StoreException(
                    message: $"Failed to update an existing application!",
                    innerException: ex
                    ).SetCallerInfo()
                     .SetOriginator(nameof(ApplicationStore))
                     .SetMethodArguments(("model", model))
                     .SetDateTime();
            }
        }

        // *******************************************************************

        /// <inheritdoc />
        public virtual async Task DeleteAsync(
            int id,
            CancellationToken cancellationToken = default
            )
        {
            try
            {
                // Validate the parameters before attempting to use them.
                Guard.Instance().ThrowIfZero(id, nameof(id));

                // Look for the corresponding model.
                var model = ApplicationRepository.AsQueryable().FirstOrDefault(
                    x => x.Id == id
                    );

                // Did we fail?
                if (null == model)
                {
                    // Panic!
                    throw new KeyNotFoundException(
                        message: $"Key: {id}"
                        );
                }

                // Defer to the repository.
                await ApplicationRepository.DeleteAsync(
                    model,
                    cancellationToken
                    ).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // Provide better context for the error.
                throw new StoreException(
                    message: $"Failed to delete an existing application!",
                    innerException: ex
                    ).SetCallerInfo()
                     .SetOriginator(nameof(ApplicationStore))
                     .SetMethodArguments(("id", id))
                     .SetDateTime();
            }
        }
        #endregion
    }
}

