using CG.Business.Repositories;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CG.Olive.Repositories
{
    /// <summary>
    /// This interface represents an object that manages the storage and retrival of 
    /// <see cref="CG.Olive.Models.Setting"/> objects.
    /// </summary>
    public interface ISettingRepository : IRepository
    {
        /// <summary>
        /// This method returns an <see cref="IQueryable{T}"/> object for 
        /// the collection of <see cref="CG.Olive.Models.Setting"/> objects maintained
        /// by this repository.
        /// </summary>
        /// <returns>A <see cref="IQueryable{T}"/> object.</returns>
        IQueryable<CG.Olive.Models.Setting> AsQueryable();

        /// <summary>
        /// This method adds a new <see cref="CG.Olive.Models.Setting"/> object
        /// to the repository.
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
        /// in the repository.
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
        /// <param name="model">The model to use for the operation.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A task to perform the operation that returns the newly
        /// updated <see cref="CG.Olive.Models.Setting"/> object.</returns>
        Task DeleteAsync(
            CG.Olive.Models.Setting model,
            CancellationToken cancellationToken = default
            );
    }
}
