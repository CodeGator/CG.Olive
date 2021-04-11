echo this script updates a local database from EFCore migrations

dotnet ef database update InitialCreate --project .\src\Data\CG.Olive.SqlServer --context "OliveDbContext" --verbose


