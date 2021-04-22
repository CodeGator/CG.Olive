using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace CG.Olive.SqlServer.Maps
{
    /// <summary>
    /// This class represents the entity mapping for the <see cref="CG.Olive.Models.Feature"/>
    /// object.
    /// </summary>
    /// <remarks>
    /// This class contains logic to build an EFCORE mapping between the <see cref="CG.Olive.Models.Feature"/>
    /// entity type and the Olive.Features table, in the database.
    /// </remarks>
    internal class FeatureMap : IEntityTypeConfiguration<CG.Olive.Models.Feature>
    {
        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <summary>
        /// This method configures the <see cref="Models.Feature"/> entity.
        /// </summary>
        /// <param name="builder">The builder to use for the operation.</param>
        public void Configure(
            EntityTypeBuilder<Models.Feature> builder
            )
        {
            builder.ToTable("Features", "Olive");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            builder.Property(e => e.Key)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(e => e.Enabled)
                .IsRequired(); 

            builder.Property(e => e.Comment)
                .HasMaxLength(255);

            builder.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(e => e.CreatedDate)
                .IsRequired()
                .HasDefaultValue(DateTime.Now);

            builder.Property(e => e.UpdatedBy)
                .HasMaxLength(50);

            builder.Property(e => e.UpdatedDate);

            builder.Property(e => e.EnvironmentId)
                .IsRequired();

            builder.HasOne(e => e.Environment)
                .WithMany()
                .HasForeignKey(e => e.EnvironmentId)
                .OnDelete(DeleteBehavior.Restrict); 

            builder.Property(e => e.ApplicationId)
                .IsRequired();

            builder.HasOne(e => e.Application)
                .WithMany()
                .HasForeignKey(e => e.ApplicationId)
                .OnDelete(DeleteBehavior.Restrict); 

            builder.HasIndex(e => new { e.Key, e.EnvironmentId, e.ApplicationId })
                .IsUnique();
        }

        #endregion
    }
}
