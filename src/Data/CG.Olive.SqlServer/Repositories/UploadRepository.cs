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
    /// This class is a SQL Server implementation of the <see cref="IUploadRepository"/>
    /// interface.
    /// </summary>
    public class UploadRepository : 
        EFCoreRepositoryBase<OliveDbContext, IOptions<OliveRepositoryOptions>>, 
        IUploadRepository
    {
        // *******************************************************************
        // Constructors.
        // *******************************************************************

        #region Constructors

        /// <summary>
        /// This constructor creates a new instance of the <see cref="UploadRepository"/>
        /// class.
        /// </summary>
        /// <param name="options">The options for the repository.</param>
        /// <param name="factory">The data-context factory for the repository.</param>
        public UploadRepository(
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
        public virtual IQueryable<CG.Olive.Models.Upload> AsQueryable()
        {
            try
            {
                // Create a context.
                var context = Factory.Create();

                // Defer to the data-context.
                var query = context.Uploads.AsQueryable()
                    .Include(x => x.Application)
                    .Include(x => x.Environment)
                    .OrderBy(x => x.Application.Name);

                // Return the results.
                return query;
            }
            catch (Exception ex)
            {
                // Provide better context for the error.
                throw new RepositoryException(
                    message: $"Failed to query for uploads!",
                    innerException: ex
                    ).SetCallerInfo()
                     .SetOriginator(nameof(UploadRepository))
                     .SetDateTime();
            }
        }

        // *******************************************************************

        /// <inheritdoc />
        public virtual async Task<CG.Olive.Models.Upload> AddAsync(
            Models.Upload model,
            CancellationToken cancellationToken = default
            )
        {
            try
            {
                // Validate the parameters before attempting to use them.
                Guard.Instance().ThrowIfNull(model, nameof(model));

                // Create a context.
                var context = Factory.Create();

                // Prevent EFCore from doing anything goofy.
                model.Application = null;
                model.Environment = null;

                // Add to the data-context.
                var entity = await context.Uploads.AddAsync(
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
                    message: $"Failed to add a new upload!",
                    innerException: ex
                    ).SetCallerInfo()
                     .SetOriginator(nameof(UploadRepository))
                     .SetMethodArguments(("model", model))
                     .SetDateTime();
            }
        }

        // *******************************************************************

        /// <inheritdoc />
        public virtual async Task<CG.Olive.Models.Upload> UpdateAsync(
            CG.Olive.Models.Upload model,
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
                var originalModel = context.Uploads.Find(
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
                    message: $"Failed to update an existing upload!",
                    innerException: ex
                    ).SetCallerInfo()
                     .SetOriginator(nameof(UploadRepository))
                     .SetMethodArguments(("model", model))
                     .SetDateTime();
            }
        }

        // *******************************************************************

        /// <inheritdoc />
        public virtual async Task DeleteAsync(
            CG.Olive.Models.Upload model,
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
                context.Uploads.Remove(model);

                // Save the changes.
                await context.SaveChangesAsync(
                    cancellationToken
                    ).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // Provide better context for the error.
                throw new RepositoryException(
                    message: $"Failed to delete an existing upload!",
                    innerException: ex
                    ).SetCallerInfo()
                     .SetOriginator(nameof(UploadRepository))
                     .SetMethodArguments(("model", model))
                     .SetDateTime();
            }            
        }

        #endregion
    }
}
