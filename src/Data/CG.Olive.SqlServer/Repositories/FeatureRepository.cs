using CG.Business.Repositories;
using CG.Linq.EFCore;
using CG.Linq.EFCore.Repositories;
using CG.Olive.Repositories;
using CG.Olive.SqlServer.Options;
using CG.Validations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CG.Olive.SqlServer.Repositories
{
    /// <summary>
    /// This class is a SQL Server implementation of the <see cref="IFeatureRepository"/>
    /// interface.
    /// </summary>
    public class FeatureRepository : 
        EFCoreRepositoryBase<OliveDbContext, IOptions<OliveRepositoryOptions>>, 
        IFeatureRepository
    {
        // *******************************************************************
        // Constructors.
        // *******************************************************************

        #region Constructors

        /// <summary>
        /// This constructor creates a new instance of the <see cref="FeatureRepository"/>
        /// class.
        /// </summary>
        /// <param name="options">The options for the repository.</param>
        /// <param name="factory">The data-context factory for the repository.</param>
        public FeatureRepository(
            IOptions<OliveRepositoryOptions> options,
            DbContextFactory<OliveDbContext> factory
            ) : base(options, factory)
        {
            
        }

        #endregion

        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <inheritdoc />
        public virtual IQueryable<CG.Olive.Models.Feature> AsQueryable()
        {
            try
            {
                // Create a context.
                var context = Factory.Create();

                // Defer to the data-context.
                var query = context.Features.AsQueryable()
                    .Include(x => x.Application)
                    .Include(x => x.Environment)
                    .OrderBy(x => x.Key);

                // Return the results.
                return query;
            }
            catch (Exception ex)
            {
                // Provide better context for the error.
                throw new RepositoryException(
                    message: $"Failed to query for features!",
                    innerException: ex
                    ).SetCallerInfo()
                     .SetOriginator(nameof(FeatureRepository))
                     .SetDateTime();
            }
        }

        // *******************************************************************

        /// <inheritdoc />
        public virtual async Task<CG.Olive.Models.Feature> AddAsync(
            Models.Feature model,
            CancellationToken cancellationToken = default
            )
        {
            try
            {
                // Validate the parameters before attempting to use them.
                Guard.Instance().ThrowIfNull(model, nameof(model));

                // Create a context.
                var context = Factory.Create();

                // Prevent EFCore from doing anything goofy with any of
                //   the associated objects.
                model.Application = null;
                model.Environment = null;

                // Add to the data-context.
                var entity = await context.Features.AddAsync(
                    model,
                    cancellationToken
                    ).ConfigureAwait(false);

                // Save the changes.
                await context.SaveChangesAsync(
                    cancellationToken
                    ).ConfigureAwait(false);

                // Return the newly added model.
                return entity.Entity;
            }
            catch (Exception ex)
            {
                // Provide better context for the error.
                throw new RepositoryException(
                    message: $"Failed to add a new feature!",
                    innerException: ex
                    ).SetCallerInfo()
                     .SetOriginator(nameof(FeatureRepository))
                     .SetMethodArguments(("model", model))
                     .SetDateTime();
            }
        }

        // *******************************************************************

        /// <inheritdoc />
        public virtual async Task<CG.Olive.Models.Feature> UpdateAsync(
            CG.Olive.Models.Feature model,
            CancellationToken cancellationToken = default
            )
        {
            try
            {
                // Validate the parameters before attempting to use them.
                Guard.Instance().ThrowIfNull(model, nameof(model));

                // Create a context.
                var context = Factory.Create();

                // Find the model in the data-context.
                var originalModel = context.Features.Find(
                    model.Id
                    );

                // Did we fail?
                if (null == originalModel)
                {
                    // Panic!
                    throw new KeyNotFoundException(
                        message: $"Key: {model.Id}"
                        );
                }

                // Update the editable properties.
                originalModel.UpdatedDate = model.UpdatedDate;
                originalModel.UpdatedBy = model.UpdatedBy;
                originalModel.ApplicationId = model.ApplicationId;
                originalModel.EnvironmentId = model.EnvironmentId;
                originalModel.Key = model.Key;
                originalModel.Value = model.Value;
                originalModel.Comment = model.Comment;

                // Save the changes.
                await context.SaveChangesAsync(
                    cancellationToken
                    ).ConfigureAwait(false);

                // Return the newly updated model.
                return originalModel;
            }
            catch (Exception ex)
            {
                // Provide better context for the error.
                throw new RepositoryException(
                    message: $"Failed to update an existing feature!",
                    innerException: ex
                    ).SetCallerInfo()
                     .SetOriginator(nameof(FeatureRepository))
                     .SetMethodArguments(("model", model))
                     .SetDateTime();
            }
        }

        // *******************************************************************

        /// <inheritdoc />
        public virtual async Task DeleteAsync(
            CG.Olive.Models.Feature model,
            CancellationToken cancellationToken = default
            )
        {
            try
            {
                // Validate the parameters before attempting to use them.
                Guard.Instance().ThrowIfNull(model, nameof(model));

                // Create a context.
                var context = Factory.Create();

                // Defer to the data-context.
                context.Features.Remove(model);

                // Save the changes.
                await context.SaveChangesAsync(
                    cancellationToken
                    ).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // Provide better context for the error.
                throw new RepositoryException(
                    message: $"Failed to delete an existing feature!",
                    innerException: ex
                    ).SetCallerInfo()
                     .SetOriginator(nameof(FeatureRepository))
                     .SetMethodArguments(("model", model))
                     .SetDateTime();
            }            
        }

        #endregion
    }
}
