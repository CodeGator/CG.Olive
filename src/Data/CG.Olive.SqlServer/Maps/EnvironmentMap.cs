using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace CG.Olive.SqlServer.Maps
{
    /// <summary>
    /// This class represents the entity mapping for the <see cref="CG.Olive.Models.Environment"/>
    /// object.
    /// </summary>
    /// <remarks>
    /// This class contains logic to build an EFCORE mapping between the <see cref="CG.Olive.Models.Environment"/>
    /// entity type and the Olive.Environments table, in the database.
    /// </remarks>
    internal class EnvironmentMap : IEntityTypeConfiguration<CG.Olive.Models.Environment>
    {
        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <summary>
        /// This method configures the <see cref="Models.Environment"/> entity.
        /// </summary>
        /// <param name="builder">The builder to use for the operation.</param>
        public void Configure(
            EntityTypeBuilder<CG.Olive.Models.Environment> builder
            )
        {
            builder.ToTable("Environments", "Olive");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id)
                .ValueGeneratedOnAdd();
                        
            builder.Property(e => e.Name)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(e => e.IsDefault)
                .HasDefaultValue(0);

            builder.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(e => e.CreatedDate)
                .IsRequired()
                .HasDefaultValue(DateTime.Now);

            builder.Property(e => e.UpdatedBy)
                .HasMaxLength(50);

            builder.Property(e => e.UpdatedDate);

            builder.HasIndex(e => e.Name).IsUnique();
        }

        #endregion
    }
}
