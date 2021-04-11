using CG.Olive.SqlServer.Maps;
using Microsoft.EntityFrameworkCore;
using System;

namespace CG.Olive.SqlServer
{
    /// <summary>
    /// This class is a data-context for the CG.Olive library.
    /// </summary>
    /// <remarks>
    /// This class contains those parts of the database that are specific to
    /// the <see cref="CG.Olive.SqlServer"/> library.
    /// </remarks>
    public class OliveDbContext : DbContext
    {
        // *******************************************************************
        // Properties.
        // *******************************************************************

        #region Properties

        /// <summary>
        /// This property contains a set a <see cref="CG.Olive.Models.Application"/>
        /// objects.
        /// </summary>
        public virtual DbSet<CG.Olive.Models.Application> Applications { get; set; }

        /// <summary>
        /// This property contains a set a <see cref="CG.Olive.Models.Environment"/>
        /// objects.
        /// </summary>
        public virtual DbSet<CG.Olive.Models.Environment> Environments { get; set; }

        /// <summary>
        /// This property contains a set a <see cref="CG.Olive.Models.Upload"/>
        /// objects.
        /// </summary>
        public virtual DbSet<CG.Olive.Models.Upload> Uploads { get; set; }

        /// <summary>
        /// This property contains a set a <see cref="CG.Olive.Models.Setting"/>
        /// objects.
        /// </summary>
        public virtual DbSet<CG.Olive.Models.Setting> Settings { get; set; }

        #endregion

        // *******************************************************************
        // Constructors.
        // *******************************************************************

        #region Constructors

        /// <summary>
        /// This constructor creates a new instance of the <see cref="OliveDbContext"/>
        /// class.
        /// </summary>
        /// <param name="options">The options to use with the data-context.</param>
        public OliveDbContext(
            DbContextOptions<OliveDbContext> options
            ) : base(options)
        {

        }

        #endregion

        // *******************************************************************
        // Protected methods.
        // *******************************************************************

        #region Protected methods

        /// <summary>
        /// This method is called to create the data model for the data-context.
        /// </summary>
        /// <param name="modelBuilder">The builder to use for the operation.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Build up the data model.
            modelBuilder.ApplyConfiguration(new ApplicationMap());
            modelBuilder.ApplyConfiguration(new EnvironmentMap());
            modelBuilder.ApplyConfiguration(new UploadMap());
            modelBuilder.ApplyConfiguration(new SettingMap());

            // Give the base class a chance.
            base.OnModelCreating(modelBuilder);
        }

        #endregion
    }
}
