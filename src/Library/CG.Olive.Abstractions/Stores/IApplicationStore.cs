using CG.Business.Stores;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CG.Olive.Stores
{
    /// <summary>
    /// This interface represents an object that manages the business logic for 
    /// <see cref="CG.Olive.Models.Application"/> objects.
    /// </summary>
    public interface IApplicationStore : IStore
    {
        /// <summary>
        /// This method returns an <see cref="IQueryable{T}"/> object for 
        /// the collection of <see cref="CG.Olive.Models.Application"/> objects maintained
        /// by this store.
        /// </summary>
        /// <returns>A <see cref="IQueryable{T}"/> object.</returns>
        IQueryable<CG.Olive.Models.Application> AsQueryable();

        /// <summary>
        /// This method adds a new <see cref="CG.Olive.Models.Application"/> object
        /// to the store.
        /// </summary>
        /// <param name="model">The model to use for the operation.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A task to perform the operation that returns the newly
        /// added <see cref="CG.Olive.Models.Application"/> object.</returns>
        Task<CG.Olive.Models.Application> AddAsync(
            CG.Olive.Models.Application model,
            CancellationToken cancellationToken = default
            );

        /// <summary>
        /// This method updates an existing <see cref="CG.Olive.Models.Application"/> object
        /// in the store.
        /// </summary>
        /// <param name="model">The model to use for the operation.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A task to perform the operation that returns the newly
        /// updated <see cref="CG.Olive.Models.Application"/> object.</returns>
        Task<CG.Olive.Models.Application> UpdateAsync(
            CG.Olive.Models.Application model,
            CancellationToken cancellationToken = default
            );

        /// <summary>
        /// This method deletes an existing <see cref="CG.Olive.Models.Application"/> object
        /// from the store.
        /// </summary>
        /// <param name="id">The identifier for the operation.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A task to perform the operation that returns the newly
        /// updated <see cref="CG.Olive.Models.Application"/> object.</returns>
        Task DeleteAsync(
            int id,
            CancellationToken cancellationToken = default
            );
    }
}
