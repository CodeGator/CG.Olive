using CG.Business.Stores;
using CG.Olive.Models;
using CG.Olive.Repositories;
using CG.Validations;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Environment = CG.Olive.Models.Environment;

namespace CG.Olive.Stores
{
    /// <summary>
    /// This class is a default implementation of the <see cref="IUploadStore"/>
    /// interface.
    /// </summary>
    public class UploadStore : StoreBase, IUploadStore
    {
        // *******************************************************************
        // Properties.
        // *******************************************************************

        #region Properties

        /// <summary>
        /// This property contains a reference to a logger.
        /// </summary>
        protected ILogger<UploadStore> Logger { get; }

        /// <summary>
        /// This property contains a reference to an upload repository.
        /// </summary>
        protected IUploadRepository UploadRepository { get; }

        #endregion

        // *******************************************************************
        // Constructors.
        // *******************************************************************

        #region Constructors

        /// <summary>
        /// This constructor creates a new instance of the <see cref="UploadStore"/>
        /// class.
        /// </summary>
        /// <param name="logger">The logger to use for the store.</param>
        /// <param name="uploadRepository">The upload repository to use
        /// with the store.</param>
        public UploadStore(
            ILogger<UploadStore> logger,
            IUploadRepository uploadRepository
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(logger, nameof(logger))
                .ThrowIfNull(uploadRepository, nameof(uploadRepository));

            // Save the references.
            Logger = logger;
            UploadRepository = uploadRepository;
        }

        #endregion

        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <inheritdoc />
        public virtual IQueryable<Upload> AsQueryable()
        {
            try
            {
                // Defer to the repository.
                var query = UploadRepository.AsQueryable();

                // Return the results.
                return query;
            }
            catch (Exception ex)
            {
                // Provide better context for the error.
                throw new StoreException(
                    message: $"Failed to query for uploads!",
                    innerException: ex
                    ).SetCallerInfo()
                     .SetOriginator(nameof(UploadStore))
                     .SetDateTime();
            }
        }

        // *******************************************************************

        /// <inheritdoc />
        public virtual async Task<Upload> AddAsync(
            Upload model,
            CancellationToken cancellationToken = default
            )
        {
            try
            {
                // Validate the parameters before attempting to use them.
                Guard.Instance().ThrowIfNull(model, nameof(model))
                    .ThrowIfDuplicate(model, nameof(model), this);

                // Defer to the repository.
                var addedModel = await UploadRepository.AddAsync(
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
                    message: $"Failed to add a new upload!",
                    innerException: ex
                    ).SetCallerInfo()
                     .SetOriginator(nameof(UploadStore))
                     .SetMethodArguments(("model", model))
                     .SetDateTime();
            }
        }

        // *******************************************************************

        /// <inheritdoc />
        public virtual async Task<Upload> UpdateAsync(
            Upload model,
            CancellationToken cancellationToken = default
            )
        {
            try
            {
                // Validate the parameters before attempting to use them.
                Guard.Instance().ThrowIfNull(model, nameof(model));

                // Defer to the repository.
                var updatedModel = await UploadRepository.UpdateAsync(
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
                    message: $"Failed to update an existing upload!",
                    innerException: ex
                    ).SetCallerInfo()
                     .SetOriginator(nameof(UploadStore))
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
                var model = UploadRepository.AsQueryable().FirstOrDefault(
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
                await UploadRepository.DeleteAsync(
                    model,
                    cancellationToken
                    ).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // Provide better context for the error.
                throw new StoreException(
                    message: $"Failed to delete an existing upload!",
                    innerException: ex
                    ).SetCallerInfo()
                     .SetOriginator(nameof(UploadStore))
                     .SetMethodArguments(("id", id))
                     .SetDateTime();
            }
        }

        // *******************************************************************

        /// <inheritdoc />
        public virtual async Task<Upload> UploadFromJsonStreamAsync(
            Stream incomingStream,
            Application application,
            Environment environment,
            string incomingFileName,
            string userName,
            CancellationToken cancellationToken = default
            )
        {
            try
            {
                // Validate the parameters before attempting to use them.
                Guard.Instance().ThrowIfNull(incomingStream, nameof(incomingStream))
                    .ThrowIfNull(application, nameof(application))
                    .ThrowIfNullOrEmpty(incomingFileName, nameof(incomingFileName))
                    .ThrowIfNullOrEmpty(userName, nameof(userName));

                // Get the JSON size.
                var length = incomingStream.Length;

                // Create a buffer.
                var buffer = new byte[length];

                // Read the JSON document into the buffer.
                var bytesRead = await incomingStream.ReadAsync(
                    buffer,
                    cancellationToken
                    ).ConfigureAwait(false);

                // Sanity check the read.
                if (bytesRead != length)
                {
                    // Panic!
                    throw new Exception(
                        $"tried to read {length} bytes, but only read {bytesRead} bytes."
                        );
                }

                // Convert the buffer to UTF8 JSON text.
                var json = Encoding.UTF8.GetString(buffer);

                // Create a model.
                var upload = new Upload()
                {
                    Json = json,
                    Size = length,
                    FileName = incomingFileName,
                    ApplicationId = application.Id,
                    EnvironmentId = environment.Id,
                    CreatedBy = userName,
                    CreatedDate = DateTime.Now
                };

                // Defer to the store.
                var newModel = await AddAsync(
                    upload,
                    cancellationToken
                    ).ConfigureAwait(false);

                // Return the results.
                return newModel;
            }
            catch (Exception ex)
            {
                // Panic!
                throw new StoreException(
                    message: "Failed to parse and import an upload!",
                    innerException: ex
                    ).SetCallerInfo()
                     .SetOriginator(nameof(UploadStore))
                     .SetMethodArguments(
                        ("application", application),
                        ("environment", environment),
                        ("incomingStream", incomingStream),
                        ("incomingFileName", incomingFileName),
                        ("userEmail", userName)
                        ).SetDateTime();
            }
        }

        #endregion
    }
}

