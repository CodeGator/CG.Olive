echo this script generates SQL scripts from EFCore migrations

dotnet ef migrations script 0 InitialCreate --project .\src\Data\CG.Olive.SqlServer --context "OliveDbContext" --verbose --output .\sql-scripts\InitialCreate.sql

dotnet ef migrations script 0 CreateIdentitySchema --project .\src\Web\CG.Olive.Web.Server --context "OliveDbContext" --verbose --output .\sql-scripts\CreateIdentitySchema .sql





