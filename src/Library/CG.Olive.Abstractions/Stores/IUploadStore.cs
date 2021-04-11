using CG.Business.Stores;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CG.Olive.Stores
{
    /// <summary>
    /// This interface represents an object that manages the business logic for 
    /// <see cref="CG.Olive.Models.Upload"/> objects.
    /// </summary>
    public interface IUploadStore : IStore
    {
        /// <summary>
        /// This method returns an <see cref="IQueryable{T}"/> object for 
        /// the collection of <see cref="CG.Olive.Models.Upload"/> objects maintained
        /// by this store.
        /// </summary>
        /// <returns>A <see cref="IQueryable{T}"/> object.</returns>
        IQueryable<CG.Olive.Models.Upload> AsQueryable();

        /// <summary>
        /// This method adds a new <see cref="CG.Olive.Models.Upload"/> object
        /// to the store.
        /// </summary>
        /// <param name="model">The model to use for the operation.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A task to perform the operation that returns the newly
        /// added <see cref="CG.Olive.Models.Upload"/> object.</returns>
        Task<CG.Olive.Models.Upload> AddAsync(
            CG.Olive.Models.Upload model,
            CancellationToken cancellationToken = default
            );

        /// <summary>
        /// This method deletes an existing <see cref="CG.Olive.Models.Upload"/> object
        /// from the store.
        /// </summary>
        /// <param name="id">The identifier for the operation.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A task to perform the operation that returns the newly
        /// updated <see cref="CG.Olive.Models.Upload"/> object.</returns>
        Task DeleteAsync(
            int id,
            CancellationToken cancellationToken = default
            );

        /// <summary>
        /// This method accepts a stream containing JSON text, an application 
        /// and an optional environment, and uploads the information to the store.
        /// </summary>
        /// <param name="incomingStream">The JSON stream to read from.</param>
        /// <param name="application">The application to use for the operation.</param>
        /// <param name="environment">The optional environment to use for the operation.</param>
        /// <param name="incomingFileName">The name of the file used for the operation.</param>
        /// <param name="userEmail">The user email to use for the operation.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A task to perform the operation, that returns the newly
        /// generated <see cref="CG.Olive.Models.Upload"/> object.</returns>
        Task<CG.Olive.Models.Upload> UploadFromJsonStreamAsync(
            Stream incomingStream,
            CG.Olive.Models.Application application,
            CG.Olive.Models.Environment environment,
            string incomingFileName,
            string userEmail,
            CancellationToken cancellationToken = default
            );
    }
}
