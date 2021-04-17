using CG.Business.Stores;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CG.Olive.Stores
{
    /// <summary>
    /// This interface represents an object that manages the business logic for 
    /// <see cref="CG.Olive.Models.Setting"/> objects.
    /// </summary>
    public interface ISettingStore : IStore
    {
        /// <summary>
        /// This method returns an <see cref="IQueryable{T}"/> object for 
        /// the collection of <see cref="CG.Olive.Models.Setting"/> objects maintained
        /// by this store.
        /// </summary>
        /// <returns>A <see cref="IQueryable{T}"/> object.</returns>
        IQueryable<CG.Olive.Models.Setting> AsQueryable();

        /// <summary>
        /// This method adds a new <see cref="CG.Olive.Models.Setting"/> object
        /// to the store.
        /// </summary>
        /// <param name="model">The model to use for the operation.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A task to perform the operation that returns the newly
        /// added <see cref="CG.Olive.Models.Setting"/> object.</returns>
        Task<CG.Olive.Models.Setting> AddAsync(
            CG.Olive.Models.Setting model,
            CancellationToken cancellationToken = default
            );

        /// <summary>
        /// This method updates an existing <see cref="CG.Olive.Models.Setting"/> object
        /// in the store.
        /// </summary>
        /// <param name="model">The model to use for the operation.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A task to perform the operation that returns the newly
        /// updated <see cref="CG.Olive.Models.Setting"/> object.</returns>
        Task<CG.Olive.Models.Setting> UpdateAsync(
            CG.Olive.Models.Setting model,
            CancellationToken cancellationToken = default
            );

        /// <summary>
        /// This method deletes an existing <see cref="CG.Olive.Models.Setting"/> object
        /// from the store.
        /// </summary>
        /// <param name="id">The identifier for the operation.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A task to perform the operation that returns the newly
        /// updated <see cref="CG.Olive.Models.Setting"/> object.</returns>
        Task DeleteAsync(
            int id,
            CancellationToken cancellationToken = default
            );

        /// <summary>
        /// This method parses through an upload and saves the results as settings.
        /// </summary>
        /// <param name="upload">The upload to use for the operation.</param>
        /// <param name="userName">The user name of the person performing the action.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A task to perform the operation that returns a count of 
        /// the number of <see cref="CG.Olive.Models.Setting"/> objects created for the upload.</returns>
        Task<long> ApplyUploadAsync(
            CG.Olive.Models.Upload upload,
            string userName,
            CancellationToken cancellationToken = default
            );

        /// <summary>
        /// This method removes all the settings for the specified upload.
        /// </summary>
        /// <param name="upload">The upload to use for the operation.</param>
        /// <param name="userName">The user name of the person performing the action.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A task to perform the operation.</returns>
        Task RollbackUploadAsync(
            CG.Olive.Models.Upload upload,
            string userName,
            CancellationToken cancellationToken = default
            );
    }
}
