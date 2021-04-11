using CG.Business.Repositories;
using CG.Linq.EFCore;
using CG.Linq.EFCore.Repositories;
using CG.Olive.Repositories;
using CG.Olive.SqlServer.Options;
using CG.Validations;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CG.Olive.SqlServer.Repositories
{
    /// <summary>
    /// This class is a SQL Server implementation of the <see cref="IApplicationRepository"/>
    /// interface.
    /// </summary>
    public class ApplicationRepository :
        EFCoreRepositoryBase<OliveDbContext, IOptions<OliveRepositoryOptions>>, 
        IApplicationRepository
    {
        // *******************************************************************
        // Constructors.
        // *******************************************************************

        #region Constructors

        /// <summary>
        /// This constructor creates a new instance of the <see cref="ApplicationRepository"/>
        /// class.
        /// </summary>
        /// <param name="options">The options for the repository.</param>
        /// <param name="factory">The data-context factory for the repository.</param>
        public ApplicationRepository(
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
        public virtual IQueryable<CG.Olive.Models.Application> AsQueryable()
        {
            try
            {
                // Create a context.
                var context = Factory.Create();

                // Defer to the data-context.
                var query = context.Applications.AsQueryable();

                // Return the results.
                return query;
            }
            catch (Exception ex)
            {
                // Provide better context for the error.
                throw new RepositoryException(
                    message: $"Failed to query for applications!",
                    innerException: ex
                    ).SetCallerInfo()
                     .SetOriginator(nameof(ApplicationRepository))
                     .SetDateTime();
            }
        }

        // *******************************************************************

        /// <inheritdoc />
        public virtual async Task<CG.Olive.Models.Application> AddAsync(
            CG.Olive.Models.Application model,
            CancellationToken cancellationToken = default
            )
        {
            try
            {
                // Validate the parameters before attempting to use them.
                Guard.Instance().ThrowIfNull(model, nameof(model));

                // Create a context.
                var context = Factory.Create();

                // Add to the data-context.
                var entity = await context.Applications.AddAsync(
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
                    message: $"Failed to add a new application!",
                    innerException: ex
                    ).SetCallerInfo()
                     .SetOriginator(nameof(ApplicationRepository))
                     .SetMethodArguments(("model", model))
                     .SetDateTime();
            }
        }

        // *******************************************************************

        /// <inheritdoc />
        public virtual async Task<CG.Olive.Models.Application> UpdateAsync(
            CG.Olive.Models.Application model,
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
                var originalModel = context.Applications.Find(
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
                originalModel.IsLocked = model.IsLocked;
                originalModel.Sid = model.Sid;
                originalModel.SKey = model.SKey;

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
                    message: $"Failed to update an existing application!",
                    innerException: ex
                    ).SetCallerInfo()
                     .SetOriginator(nameof(ApplicationRepository))
                     .SetMethodArguments(("model", model))
                     .SetDateTime();
            }
        }

        // *******************************************************************

        /// <inheritdoc />
        public virtual async Task DeleteAsync(
            CG.Olive.Models.Application model,
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
                context.Applications.Remove(model);

                // Save the changes.
                await context.SaveChangesAsync(
                    cancellationToken
                    ).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // Provide better context for the error.
                throw new RepositoryException(
                    message: $"Failed to delete an existing application!",
                    innerException: ex
                    ).SetCallerInfo()
                     .SetOriginator(nameof(ApplicationRepository))
                     .SetMethodArguments(("model", model))
                     .SetDateTime();
            }            
        }

        #endregion
    }
}
