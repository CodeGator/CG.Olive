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
    /// This class is a SQL Server implementation of the <see cref="ISettingRepository"/>
    /// interface.
    /// </summary>
    public class SettingRepository : 
        EFCoreRepositoryBase<OliveDbContext, IOptions<OliveRepositoryOptions>>, 
        ISettingRepository
    {
        // *******************************************************************
        // Constructors.
        // *******************************************************************

        #region Constructors

        /// <summary>
        /// This constructor creates a new instance of the <see cref="SettingRepository"/>
        /// class.
        /// </summary>
        /// <param name="options">The options for the repository.</param>
        /// <param name="factory">The data-context factory for the repository.</param>
        public SettingRepository(
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
        public virtual IQueryable<CG.Olive.Models.Setting> AsQueryable()
        {
            try
            {
                // Create a context.
                var context = Factory.Create();

                // Defer to the data-context.
                var query = context.Settings.AsQueryable()
                    .Include(x => x.Application)
                    .Include(x => x.Environment)
                    .Include(x => x.Upload)
                    .OrderBy(x => x.Key);

                // Return the results.
                return query;
            }
            catch (Exception ex)
            {
                // Provide better context for the error.
                throw new RepositoryException(
                    message: $"Failed to query for settings!",
                    innerException: ex
                    ).SetCallerInfo()
                     .SetOriginator(nameof(SettingRepository))
                     .SetDateTime();
            }
        }

        // *******************************************************************

        /// <inheritdoc />
        public virtual async Task<CG.Olive.Models.Setting> AddAsync(
            Models.Setting model,
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
                model.Upload = null;

                // Add to the data-context.
                var entity = await context.Settings.AddAsync(
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
                    message: $"Failed to add a new setting!",
                    innerException: ex
                    ).SetCallerInfo()
                     .SetOriginator(nameof(SettingRepository))
                     .SetMethodArguments(("model", model))
                     .SetDateTime();
            }
        }

        // *******************************************************************

        /// <inheritdoc />
        public virtual async Task<CG.Olive.Models.Setting> UpdateAsync(
            CG.Olive.Models.Setting model,
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
                var originalModel = context.Settings.Find(
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
                originalModel.IsSecret = model.IsSecret;

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
                    message: $"Failed to update an existing setting!",
                    innerException: ex
                    ).SetCallerInfo()
                     .SetOriginator(nameof(SettingRepository))
                     .SetMethodArguments(("model", model))
                     .SetDateTime();
            }
        }

        // *******************************************************************

        /// <inheritdoc />
        public virtual async Task DeleteAsync(
            CG.Olive.Models.Setting model,
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
                context.Settings.Remove(model);

                // Save the changes.
                await context.SaveChangesAsync(
                    cancellationToken
                    ).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // Provide better context for the error.
                throw new RepositoryException(
                    message: $"Failed to delete an existing setting!",
                    innerException: ex
                    ).SetCallerInfo()
                     .SetOriginator(nameof(SettingRepository))
                     .SetMethodArguments(("model", model))
                     .SetDateTime();
            }            
        }

        // *******************************************************************

        /// <inheritdoc />
        public virtual async Task RollbackUploadAsync(
            int uploadId,
            CancellationToken cancellationToken = default
            )
        {
            try
            {
                // Validate the parameters before attempting to use them.
                Guard.Instance().ThrowIfZero(uploadId, nameof(uploadId));

                // Create a context.
                var context = Factory.Create();

                // Defer to the data-context.
                await context.RollbackUploadAsync(
                    uploadId,
                    cancellationToken
                    ).ConfigureAwait(false);

                // Save the changes.
                await context.SaveChangesAsync(
                    cancellationToken
                    ).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // Provide better context for the error.
                throw new RepositoryException(
                    message: $"Failed to rollback settings for an upload!",
                    innerException: ex
                    ).SetCallerInfo()
                     .SetOriginator(nameof(SettingRepository))
                     .SetMethodArguments(("uploadId", uploadId))
                     .SetDateTime();
            }
        }

        #endregion
    }
}
