using CG.Business.Stores;
using CG.DataAnnotations;
using CG.IO;
using CG.Olive.Models;
using CG.Olive.Repositories;
using CG.Validations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CG.Olive.Stores
{
    /// <summary>
    /// This class is a default implementation of the <see cref="ISettingStore"/>
    /// interface.
    /// </summary>
    public class SettingStore : StoreBase, ISettingStore
    {
        // *******************************************************************
        // Properties.
        // *******************************************************************

        #region Properties

        /// <summary>
        /// This property contains a reference to a logger.
        /// </summary>
        protected ILogger<SettingStore> Logger { get; }

        /// <summary>
        /// This model contains a reference to an upload repository.
        /// </summary>
        protected ISettingRepository SettingRepository { get; }

        #endregion

        // *******************************************************************
        // Constructors.
        // *******************************************************************

        #region Constructors

        /// <summary>
        /// This constructor creates a new instance of the <see cref="SettingStore"/>
        /// class.
        /// </summary>
        /// <param name="logger">The logger to use for the store.</param>
        /// <param name="uploadRepository">The upload repository to use
        /// with the store.</param>
        public SettingStore(
            ILogger<SettingStore> logger,
            ISettingRepository uploadRepository
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(logger, nameof(logger))
                .ThrowIfNull(uploadRepository, nameof(uploadRepository));

            // Save the references.
            Logger = logger;
            SettingRepository = uploadRepository;
        }

        #endregion

        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <inheritdoc />
        public virtual IQueryable<Setting> AsQueryable()
        {
            try
            {
                // Defer to the repository.
                var query = SettingRepository.AsQueryable();

                // Return the results.
                return query;
            }
            catch (Exception ex)
            {
                // Provide better context for the error.
                throw new StoreException(
                    message: $"Failed to query for settings!",
                    innerException: ex
                    ).SetCallerInfo()
                     .SetOriginator(nameof(SettingStore))
                     .SetDateTime();
            }
        }

        // *******************************************************************

        /// <inheritdoc />
        public virtual async Task<Setting> AddAsync(
            Setting model,
            CancellationToken cancellationToken = default
            )
        {
            try
            {
                // Validate the parameters before attempting to use them.
                Guard.Instance().ThrowIfNull(model, nameof(model));

                // Defer to the repository.
                var addedModel = await SettingRepository.AddAsync(
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
                    message: $"Failed to add a new setting!",
                    innerException: ex
                    ).SetCallerInfo()
                     .SetOriginator(nameof(SettingStore))
                     .SetMethodArguments(("model", model))
                     .SetDateTime();
            }
        }

        // *******************************************************************

        /// <inheritdoc />
        public virtual async Task<Setting> UpdateAsync(
            Setting model,
            CancellationToken cancellationToken = default
            )
        {
            try
            {
                // Validate the parameters before attempting to use them.
                Guard.Instance().ThrowIfNull(model, nameof(model));

                // Defer to the repository.
                var updatedModel = await SettingRepository.UpdateAsync(
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
                    message: $"Failed to update an existing setting!",
                    innerException: ex
                    ).SetCallerInfo()
                     .SetOriginator(nameof(SettingStore))
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
                var model = SettingRepository.AsQueryable()
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
                await SettingRepository.DeleteAsync(
                    model,
                    cancellationToken
                    ).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // Provide better context for the error.
                throw new StoreException(
                    message: $"Failed to delete an existing setting!",
                    innerException: ex
                    ).SetCallerInfo()
                     .SetOriginator(nameof(SettingStore))
                     .SetMethodArguments(("id", id))
                     .SetDateTime();
            }
        }

        // *******************************************************************

        /// <inheritdoc />
        public virtual async Task<long> ApplyUploadAsync(
            Upload upload,
            string userName,
            CancellationToken cancellationToken = default
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(upload, nameof(upload))
                .ThrowIfNullOrEmpty(userName, nameof(userName));

            // Create and configure a builder.
            var builder = new ConfigurationBuilder();
            builder.AddJsonStream(new StringStream(upload.Json));

            // Create a configuration object.
            var configuration = builder.Build();

            var settingsCount = 0L;
            var errors = new List<Exception>();

            // Loop through the data.
            foreach (var kvp in configuration.AsEnumerable())
            {
                try
                {
                    // Create the model.
                    var model = new Setting()
                    {
                        Key = kvp.Key,
                        Value = kvp.Value,
                        UploadId = upload.Id,
                        ApplicationId = upload.ApplicationId,
                        EnvironmentId = upload.EnvironmentId,
                        CreatedBy = upload.CreatedBy = userName,
                        CreatedDate = DateTime.Now
                    };

                    // Save the model.
                    var setting = await AddAsync(
                        model,
                        cancellationToken
                        ).ConfigureAwait(false);

                    // Update the count.
                    settingsCount++;
                }
                catch (Exception ex)
                {
                    // Add the error to the collection.
                    errors.Add(ex);
                }
            }

            // Were there errors?
            if (errors.Any())
            {
                // Panic!
                throw new StoreException(
                    message: "Failed to parse the upload and save the settings!",
                    innerException: new AggregateException(errors)
                    ).SetCallerInfo()
                     .SetOriginator(nameof(SettingStore))
                     .SetMethodArguments(
                        ("upload", upload),
                        ("userName", userName)
                        ).SetDateTime();
            }

            // Return the results.
            return settingsCount;
        }

        // *******************************************************************

        /// <inheritdoc />
        public virtual async Task RollbackUploadAsync(
            CG.Olive.Models.Upload upload,
            string userName,
            CancellationToken cancellationToken = default
            )
        {
            try
            {
                // Validate the parameters before attempting to use them.
                Guard.Instance().ThrowIfNull(upload, nameof(upload))
                    .ThrowIfNullOrEmpty(userName, nameof(userName));

                // Defer to the repository.
                await SettingRepository.RollbackUploadAsync(
                    upload.Id,
                    cancellationToken
                    ).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // Provide better context for the error.
                throw new StoreException(
                    message: $"Failed to rollback settings for an upload!",
                    innerException: ex
                    ).SetCallerInfo()
                     .SetOriginator(nameof(SettingStore))
                     .SetMethodArguments(
                        ("upload", upload),
                        ("userName", userName)
                        ).SetDateTime();
            }
        }

        #endregion
    }
}

