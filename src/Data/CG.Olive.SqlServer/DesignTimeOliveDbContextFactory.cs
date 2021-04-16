using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;

namespace CG.Olive.SqlServer
{
    /// <summary>
    /// This class is a default implementation of the <see cref="IDesignTimeDbContextFactory{OliveDbContext}"/>
    /// interface, used only for local, dev migrations.
    /// </summary>
    /// <remarks>
    /// This class contains a factory used by EFCORE to create data-context 
    /// instances during migration related operations. 
    /// </remarks>
    public class DesignTimeOliveDbContextFactory : IDesignTimeDbContextFactory<OliveDbContext>
    {
        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <summary>
        /// This method creates a new <see cref="OliveDbContext"/> instance.
        /// </summary>
        /// <param name="args">Optional arguments.</param>
        /// <returns>A <see cref="OliveDbContext"/> instance.</returns>
        public OliveDbContext CreateDbContext(string[] args)
        {
#if DEBUG
            // Create the builder.
            var optionsBuilder = new DbContextOptionsBuilder<OliveDbContext>();

            // Hard coded because we only use this for local, development
            //   related migrations - well, and also because we can't pass
            //   in any configuration data to the ctor. *shrugs* I don't know,
            //   go talk to the EFCore team at Microsoft.
            optionsBuilder.UseSqlServer("Server=.;Database=CG.Olive.Web;Trusted_Connection=True;MultipleActiveResultSets=true");

            // Create the and return the data-context.
            return new OliveDbContext(optionsBuilder.Options);
#else
            return null;  // never, never, never in production.
#endif
        }

        #endregion
    }
}
