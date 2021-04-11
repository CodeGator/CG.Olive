using CG.Validations;
using System;
using System.Linq;

namespace CG.Olive.SqlServer
{
    /// <summary>
    /// This class contains extension methods related to the <see cref="OliveDbContext"/>
    /// type.
    /// </summary>
    /// <remarks>
    /// This class contains <see cref="OliveDbContext"/> related operations that should
    /// only be called from within the <see cref="CG.Olive.SqlServer"/> library.
    /// </remarks>
    internal static partial class OliveDbContextExtensions
    {
        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <summary>
        /// This method applies seed data to the specified data-context.
        /// </summary>
        /// <param name="context">The data-context to use for the operation.</param>
        public static void ApplySeedData(
            this OliveDbContext context
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(context, nameof(context));

            // Add data to the tables.
            context.SeedApplications();
            context.SeedEnvironments();
            //context.SeedUploads();
            //context.SeedSettings();
        }

        #endregion

        // *******************************************************************
        // Private methods.
        // *******************************************************************

        #region Private methods

        /// <summary>
        /// This method applies seed data to the Applications table.
        /// </summary>
        /// <param name="context">The data-context to use for the operation.</param>
        private static void SeedApplications(
            this OliveDbContext context
            )
        {
            // Don't seed an already populated table.
            if (true == context.Applications.Any())
            {
                return;
            }

            // Add data to the table.
            context.AddRange(new Models.Application[]
            {
                new Models.Application()
                {
                    Name = "CG.Obsidian.Web",
                    SKey = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
                    Sid = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
                    CreatedBy = "seed",
                    CreatedDate = DateTime.Now
                },
                new Models.Application()
                {
                    Name = "CG.Coral.Web",
                    SKey = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
                    Sid = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
                    CreatedBy = "seed",
                    CreatedDate = DateTime.Now
                },
                new Models.Application()
                {
                    Name = "CG.Beryl.Web",
                    SKey = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
                    Sid = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
                    CreatedBy = "seed",
                    CreatedDate = DateTime.Now
                },
                new Models.Application()
                {
                    Name = "CG.Olive.Web",
                    SKey = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
                    Sid = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
                    CreatedBy = "seed",
                    CreatedDate = DateTime.Now
                }
            }); ;

            // Save the changes.
            context.SaveChanges();
        }

        // *******************************************************************

        /// <summary>
        /// This method applies seed data to the Environments table.
        /// </summary>
        /// <param name="context">The data-context to use for the operation.</param>
        private static void SeedEnvironments(
            this OliveDbContext context
            )
        {
            // Don't seed an already populated table.
            if (true == context.Environments.Any())
            {
                return;
            }

            // Add data to the table.
            context.AddRange(new Models.Environment[]
            {
                new Models.Environment()
                {
                    Name = "Development",
                    CreatedBy = "seed",
                    CreatedDate = DateTime.Now
                },
                new Models.Environment()
                {
                    Name = "QA",
                    CreatedBy = "seed",
                    CreatedDate = DateTime.Now
                },
                new Models.Environment()
                {
                    Name = "Production",
                    CreatedBy = "seed",
                    CreatedDate = DateTime.Now
                }
            });

            // Save the changes.
            context.SaveChanges();
        }

        // *******************************************************************

        /// <summary>
        /// This method applies seed data to the Uploads table.
        /// </summary>
        /// <param name="context">The data-context to use for the operation.</param>
        private static void SeedUploads(
            this OliveDbContext context
            )
        {
            // Don't seed an already populated table.
            if (true == context.Uploads.Any())
            {
                return;
            }

            // Add data to the table.
            context.AddRange(new Models.Upload[]
            {
                new Models.Upload()
                {
                    FileName = "appSettings.json",
                    Size = 4,
                    Json = "{  }",
                    ApplicationId = context.Applications.First(x => x.Name == "CG.Beryl.Web").Id,
                    EnvironmentId = context.Environments.First(x => x.Name == "Production").Id,
                    CreatedBy = "seed",
                    CreatedDate = DateTime.Now
                },
                new Models.Upload()
                {
                    FileName = "appSettings.Development.json",
                    Size = 4,
                    Json = "{  }",
                    ApplicationId = context.Applications.First(x => x.Name == "CG.Beryl.Web").Id,
                    EnvironmentId = context.Environments.First(x => x.Name == "Development").Id,
                    CreatedBy = "seed",
                    CreatedDate = DateTime.Now
                },
                new Models.Upload()
                {
                    FileName = "appSettings.json",
                    Size = 4,
                    Json = "{  }",
                    ApplicationId = context.Applications.First(x => x.Name == "CG.Coral.Web").Id,
                    EnvironmentId = context.Environments.First(x => x.Name == "Production").Id,
                    CreatedBy = "seed",
                    CreatedDate = DateTime.Now
                },
                new Models.Upload()
                {
                    FileName = "appSettings.Development.json",
                    Size = 4,
                    Json = "{  }",
                    ApplicationId = context.Applications.First(x => x.Name == "CG.Coral.Web").Id,
                    EnvironmentId = context.Environments.First(x => x.Name == "Development").Id,
                    CreatedBy = "seed",
                    CreatedDate = DateTime.Now
                },
                new Models.Upload()
                {
                    FileName = "appSettings.json",
                    Size = 4,
                    Json = "{  }",
                    ApplicationId = context.Applications.First(x => x.Name == "CG.Obsidian.Web").Id,
                    EnvironmentId = context.Environments.First(x => x.Name == "Production").Id,
                    CreatedBy = "seed",
                    CreatedDate = DateTime.Now
                },
                new Models.Upload()
                {
                    FileName = "appSettings.Development.json",
                    Size = 4,
                    Json = "{  }",
                    ApplicationId = context.Applications.First(x => x.Name == "CG.Obsidian.Web").Id,
                    EnvironmentId = context.Environments.First(x => x.Name == "Development").Id,
                    CreatedBy = "seed",
                    CreatedDate = DateTime.Now
                }
            });

            // Save the changes.
            context.SaveChanges();
        }

        // *******************************************************************

        /// <summary>
        /// This method applies seed data to the Settings table.
        /// </summary>
        /// <param name="context">The data-context to use for the operation.</param>
        private static void SeedSettings(
            this OliveDbContext context
            )
        {
            // Don't seed an already populated table.
            if (true == context.Settings.Any())
            {
                return;
            }

            // Add data to the table.
            context.Add(new Models.Setting()
            {
                Key = "A",
                Value = null,
                ApplicationId = context.Applications.First(x => x.Name == "CG.Obsidian.Web").Id,
                EnvironmentId = context.Environments.First(x => x.Name == "Production").Id,
                CreatedBy = "seed",
                CreatedDate = DateTime.Now
            });

            // Save the changes.
            context.SaveChanges();
        }

        #endregion
    }
}
