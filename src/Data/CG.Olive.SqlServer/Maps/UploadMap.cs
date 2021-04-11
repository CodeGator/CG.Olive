using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace CG.Olive.SqlServer.Maps
{
    /// <summary>
    /// This class represents the entity mapping for the <see cref="CG.Olive.Models.Upload"/>
    /// object.
    /// </summary>
    /// <remarks>
    /// This class contains logic to build an EFCORE mapping between the <see cref="CG.Olive.Models.Upload"/>
    /// entity type and the Olive.Uploads table, in the database.
    /// </remarks>
    internal class UploadMap : IEntityTypeConfiguration<CG.Olive.Models.Upload>
    {
        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <summary>
        /// This method configures the <see cref="CG.Olive.Models.Upload"/> entity.
        /// </summary>
        /// <param name="builder">The builder to use for the operation.</param>
        public void Configure(
            EntityTypeBuilder<CG.Olive.Models.Upload> builder
            )
        {
            builder.ToTable("Uploads", "Olive");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            builder.Property(e => e.FileName)
                .HasMaxLength(260)
                .IsRequired();

            builder.Property(e => e.Json)
                .IsRequired();

            builder.Property(e => e.Size)
                .IsRequired();

            builder.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(e => e.CreatedDate)
                .IsRequired()
                .HasDefaultValue(DateTime.Now);

            builder.Property(e => e.UpdatedBy)
                .HasMaxLength(50);

            builder.Property(e => e.UpdatedDate);

            builder.Property(e => e.EnvironmentId);
            builder.HasOne(e => e.Environment)
                .WithMany()
                .HasForeignKey(e => e.EnvironmentId)
                .OnDelete(DeleteBehavior.Restrict); 

            builder.Property(e => e.ApplicationId);
            builder.HasOne(e => e.Application)
                .WithMany()
                .HasForeignKey(e => e.ApplicationId)
                .OnDelete(DeleteBehavior.Restrict); 

            builder.HasIndex(e => new { e.ApplicationId, e.EnvironmentId })
                .IsUnique();
        }

        #endregion
    }
}
