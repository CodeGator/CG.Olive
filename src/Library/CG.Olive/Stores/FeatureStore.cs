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
    /// This class is a default implementation of the <see cref="IFeatureStore"/>
    /// interface.
    /// </summary>
    public class FeatureStore : StoreBase, IFeatureStore
    {
        // *******************************************************************
        // Properties.
        // *******************************************************************

        #region Properties

        /// <summary>
        /// This property contains a reference to a logger.
        /// </summary>
        protected ILogger<FeatureStore> Logger { get; }

        /// <summary>
        /// This model contains a reference to an feature repository.
        /// </summary>
        protected IFeatureRepository FeatureRepository { get; }

        #endregion

        // *******************************************************************
        // Constructors.
        // *******************************************************************

        #region Constructors

        /// <summary>
        /// This constructor creates a new instance of the <see cref="FeatureStore"/>
        /// class.
        /// </summary>
        /// <param name="logger">The logger to use for the store.</param>
        /// <param name="featureRepository">The feature repository to use
        /// with the store.</param>
        public FeatureStore(
            ILogger<FeatureStore> logger,
            IFeatureRepository featureRepository
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(logger, nameof(logger))
                .ThrowIfNull(featureRepository, nameof(featureRepository));

            // Save the references.
            Logger = logger;
            FeatureRepository = featureRepository;
        }

        #endregion

        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <inheritdoc />
        public virtual IQueryable<Feature> AsQueryable()
        {
            try
            {
                // Defer to the repository.
                var query = FeatureRepository.AsQueryable();

                // Return the results.
                return query;
            }
            catch (Exception ex)
            {
                // Provide better context for the error.
                throw new StoreException(
                    message: $"Failed to query for features!",
                    innerException: ex
                    ).SetCallerInfo()
                     .SetOriginator(nameof(FeatureStore))
                     .SetDateTime();
            }
        }

        // *******************************************************************

        /// <inheritdoc />
        public virtual async Task<Feature> AddAsync(
            Feature model,
            CancellationToken cancellationToken = default
            )
        {
            try
            {
                // Validate the parameters before attempting to use them.
                Guard.Instance().ThrowIfNull(model, nameof(model));

                // Defer to the repository.
                var addedModel = await FeatureRepository.AddAsync(
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
                    message: $"Failed to add a new feature!",
                    innerException: ex
                    ).SetCallerInfo()
                     .SetOriginator(nameof(FeatureStore))
                     .SetMethodArguments(("model", model))
                     .SetDateTime();
            }
        }

        // *******************************************************************

        /// <inheritdoc />
        public virtual async Task<Feature> UpdateAsync(
            Feature model,
            CancellationToken cancellationToken = default
            )
        {
            try
            {
                // Validate the parameters before attempting to use them.
                Guard.Instance().ThrowIfNull(model, nameof(model));

                // Defer to the repository.
                var updatedModel = await FeatureRepository.UpdateAsync(
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
                    message: $"Failed to update an existing feature!",
                    innerException: ex
                    ).SetCallerInfo()
                     .SetOriginator(nameof(FeatureStore))
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
                var model = FeatureRepository.AsQueryable()
                    .FirstOrDefault(x => x.Id == id);

                // Did we fail?
                if (null == model)
                {
                    // Panic!
                    throw new KeyNotFoundException(
                        message: $"Key: {id}"
                        );
                }

                // Defer to the repository.
                await FeatureRepository.DeleteAsync(
                    model,
                    cancellationToken
                    ).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // Provide better context for the error.
                throw new StoreException(
                    message: $"Failed to delete an existing feature!",
                    innerException: ex
                    ).SetCallerInfo()
                     .SetOriginator(nameof(FeatureStore))
                     .SetMethodArguments(("id", id))
                     .SetDateTime();
            }
        }

        #endregion
    }
}

