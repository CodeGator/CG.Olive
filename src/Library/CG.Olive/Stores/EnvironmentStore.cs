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

using Environment = CG.Olive.Models.Environment;

namespace CG.Olive.Stores
{
    /// <summary>
    /// This class is a default implementation of the <see cref="IEnvironmentStore"/>
    /// interface.
    /// </summary>
    public class EnvironmentStore : StoreBase, IEnvironmentStore
    {
        // *******************************************************************
        // Properties.
        // *******************************************************************

        #region Properties

        /// <summary>
        /// This property contains a reference to a logger.
        /// </summary>
        protected ILogger<EnvironmentStore> Logger { get; }

        /// <summary>
        /// This property contains a reference to an environment repository.
        /// </summary>
        protected IEnvironmentRepository EnvironmentRepository { get; }

        #endregion

        // *******************************************************************
        // Constructors.
        // *******************************************************************

        #region Constructors

        /// <summary>
        /// This constructor creates a new instance of the <see cref="EnvironmentStore"/>
        /// class.
        /// </summary>
        /// <param name="logger">The logger to use for the store.</param>
        /// <param name="environmentRepository">The environment repository to use
        /// with the store.</param>
        public EnvironmentStore(
            ILogger<EnvironmentStore> logger,
            IEnvironmentRepository environmentRepository
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(logger, nameof(logger))
                .ThrowIfNull(environmentRepository, nameof(environmentRepository));

            // Save the references.
            Logger = logger;
            EnvironmentRepository = environmentRepository;
        }

        #endregion

        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <inheritdoc />
        public virtual IQueryable<Environment> AsQueryable()
        {
            try
            {
                // Defer to the repository.
                var query = EnvironmentRepository.AsQueryable();

                // Return the results.
                return query;
            }
            catch (Exception ex)
            {
                // Provide better context for the error.
                throw new StoreException(
                    message: $"Failed to query for environments!",
                    innerException: ex
                    ).SetCallerInfo()
                     .SetOriginator(nameof(EnvironmentStore))
                     .SetDateTime();
            }
        }

        // *******************************************************************

        /// <inheritdoc />
        public virtual async Task<Environment> AddAsync(
            Environment model,
            CancellationToken cancellationToken = default
            )
        {
            try
            {
                // Validate the parameters before attempting to use them.
                Guard.Instance().ThrowIfNull(model, nameof(model))
                    .ThrowIfNotInAddableState(model, nameof(model));

                // First environment is automatically the default, until it isn't.
                if (AsQueryable().Count() <= 1)
                {
                    model.IsDefault = true;
                }

                // Is the default flag set?
                if (model.IsDefault)
                {
                    // If we get here then someone is trying to set the 
                    //   "IsDefault" flag. Since that flag can only be 
                    //   set once for any given row, we'll first clear
                    //   any existing flags, in the db, before we try
                    //   to save the model with the set flag.

                    // Defer to the repository.
                    await EnvironmentRepository.ClearDefault(
                        model.CreatedBy,
                        cancellationToken
                        ).ConfigureAwait(false);
                }

                // Defer to the repository.
                var addedModel = await EnvironmentRepository.AddAsync(
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
                    message: $"Failed to add a new environment!",
                    innerException: ex
                    ).SetCallerInfo()
                     .SetOriginator(nameof(EnvironmentStore))
                     .SetMethodArguments(("model", model))
                     .SetDateTime();
            }
        }

        // *******************************************************************

        /// <inheritdoc />
        public virtual async Task<Environment> UpdateAsync(
            Environment model,
            CancellationToken cancellationToken = default
            )
        {
            try
            {
                // Validate the parameters before attempting to use them.
                Guard.Instance().ThrowIfNull(model, nameof(model))
                    .ThrowIfNotInUpdateableState(model, nameof(model));

                // First environment is automatically the default, until it isn't.
                if (AsQueryable().Count() <= 1)
                {
                    model.IsDefault = true;
                }

                // Is the default flag set?
                if (model.IsDefault)
                {
                    // If we get here then someone is trying to set the 
                    //   "IsDefault" flag. Since that flag can only be 
                    //   set once for any given row, we'll first clear
                    //   any existing flags, in the db, before we try
                    //   to save the model with the set flag.

                    // Defer to the repository.
                    await EnvironmentRepository.ClearDefault(
                        model.UpdatedBy,
                        cancellationToken
                        ).ConfigureAwait(false);
                }

                // Defer to the repository.
                var updatedModel = await EnvironmentRepository.UpdateAsync(
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
                    message: $"Failed to update an existing environment!",
                    innerException: ex
                    ).SetCallerInfo()
                     .SetOriginator(nameof(EnvironmentStore))
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
                var model = EnvironmentRepository.AsQueryable().FirstOrDefault(
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
                await EnvironmentRepository.DeleteAsync(
                    model,
                    cancellationToken
                    ).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // Provide better context for the error.
                throw new StoreException(
                    message: $"Failed to delete an existing environment!",
                    innerException: ex
                    ).SetCallerInfo()
                     .SetOriginator(nameof(EnvironmentStore))
                     .SetMethodArguments(("id", id))
                     .SetDateTime();
            }
        }
        #endregion
    }
}

