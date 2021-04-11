echo this script creates EFCore migrations

dotnet ef migrations add InitialCreate --project .\src\Data\CG.Olive.SqlServer --context "OliveDbContext" --verbose
pause