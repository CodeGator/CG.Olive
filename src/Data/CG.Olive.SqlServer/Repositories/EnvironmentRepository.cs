using CG.Business.Repositories;
using CG.Linq.EFCore;
using CG.Linq.EFCore.Repositories;
using CG.Olive.Repositories;
using CG.Olive.SqlServer.Options;
using CG.Validations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CG.Olive.SqlServer.Repositories
{
    /// <summary>
    /// This class is a SQL server implementation of the <see cref="IEnvironmentRepository"/>
    /// interface.
    /// </summary>
    public class EnvironmentRepository :
        EFCoreRepositoryBase<OliveDbContext, IOptions<OliveRepositoryOptions>>, 
        IEnvironmentRepository
    {
        // *******************************************************************
        // Constructors.
        // *******************************************************************

        #region Constructors

        /// <summary>
        /// This constructor creates a new instance of the <see cref="EnvironmentRepository"/>
        /// class.
        /// </summary>
        /// <param name="options">The options for the repository.</param>
        /// <param name="factory">The data-context factory for the repository.</param>
        public EnvironmentRepository(
            IOptions<OliveRepositoryOptions> options,
            IDbContextFactory<OliveDbContext> factory
            ) : base(options, factory)
        {

        }

        #endregion

        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <inheritdoc />
        public virtual IQueryable<CG.Olive.Models.Environment> AsQueryable()
        {
            try
            {
                // Create a context.
                var context = Factory.CreateDbContext();

                // Defer to the data-context.
                var query = context.Environments.AsQueryable();

                // Return the results.
                return query;
            }
            catch (Exception ex)
            {
                // Provide better context for the error.
                throw new RepositoryException(
                    message: $"Failed to query for environments!",
                    innerException: ex
                    ).SetCallerInfo()
                     .SetOriginator(nameof(EnvironmentRepository))
                     .SetDateTime();
            }
        }

        // *******************************************************************

        /// <inheritdoc />
        public virtual async Task<CG.Olive.Models.Environment> AddAsync(
            CG.Olive.Models.Environment model,
            CancellationToken cancellationToken = default
            )
        {
            try
            {
                // Validate the parameters before attempting to use them.
                Guard.Instance().ThrowIfNull(model, nameof(model));

                // Create a context.
                var context = Factory.CreateDbContext();

                // Add to the data-context.
                var entity = await context.Environments.AddAsync(
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
                    message: $"Failed to add a new environment!",
                    innerException: ex
                    ).SetCallerInfo()
                     .SetOriginator(nameof(EnvironmentRepository))
                     .SetMethodArguments(("model", model))
                     .SetDateTime();
            }
        }

        // *******************************************************************

        /// <inheritdoc />
        public virtual async Task<CG.Olive.Models.Environment> UpdateAsync(
            CG.Olive.Models.Environment model,
            CancellationToken cancellationToken = default
            )
        {
            try
            {
                // Validate the parameters before attempting to use them.
                Guard.Instance().ThrowIfNull(model, nameof(model));

                // Create a context.
                var context = Factory.CreateDbContext();

                // Find the model in the data-context.
                var originalModel = context.Environments.Find(
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
                originalModel.Name = model.Name;
                originalModel.IsDefault = model.IsDefault;

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
                    message: $"Failed to update an existing model!",
                    innerException: ex
                    ).SetCallerInfo()
                     .SetOriginator(nameof(EnvironmentRepository))
                     .SetMethodArguments(("model", model))
                     .SetDateTime();
            }
        }

        // *******************************************************************

        /// <inheritdoc />
        public virtual async Task DeleteAsync(
            CG.Olive.Models.Environment model,
            CancellationToken cancellationToken = default
            )
        {
            try
            {
                // Validate the parameters before attempting to use them.
                Guard.Instance().ThrowIfNull(model, nameof(model));

                // Create a context.
                var context = Factory.CreateDbContext();

                // Defer to the data-context.
                context.Environments.Remove(model);

                // Save the changes.
                await context.SaveChangesAsync(
                    cancellationToken
                    ).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // Provide better context for the error.
                throw new RepositoryException(
                    message: $"Failed to delete an existing environment!",
                    innerException: ex
                    ).SetCallerInfo()
                     .SetOriginator(nameof(EnvironmentRepository))
                     .SetMethodArguments(("model", model))
                     .SetDateTime();
            }            
        }

        // *******************************************************************

        /// <inheritdoc />
        public virtual async Task ClearDefault(
            string updatedBy,
            CancellationToken cancellationToken = default
            )
        {
            try
            {
                // Validate the parameters before attempting to use them.
                Guard.Instance().ThrowIfNullOrEmpty(updatedBy, nameof(updatedBy));

                // Create a context.
                var context = Factory.CreateDbContext();

                // Find the model in the data-context (if there is one).
                var originalModel = context.Environments.FirstOrDefault(
                    x => x.IsDefault == true
                    );

                // Did we fail?
                if (null == originalModel)
                {
                    // Nothing to do.
                    return;
                }

                // Update the editable properties.
                originalModel.UpdatedDate = DateTime.Now;
                originalModel.UpdatedBy = updatedBy;
                originalModel.IsDefault = false;

                // Save the changes.
                await context.SaveChangesAsync(
                    cancellationToken
                    ).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // Provide better context for the error.
                throw new RepositoryException(
                    message: $"Failed to clear the IsDefault flag!",
                    innerException: ex
                    ).SetCallerInfo()
                     .SetOriginator(nameof(EnvironmentRepository))
                     .SetMethodArguments(("updatedBy", updatedBy))
                     .SetDateTime();
            }
        }

        #endregion
    }
}
