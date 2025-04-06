@echo off
REM This script will execute EF Core migration commands within a single project.

REM --- Configuration ---
SET ProjectName=WindTurbineApi
SET DbContextNamespace=WindTurbineApi.Infrastructure.Persistence.ApplicationDbContext
SET OutputDir=.\Infrastructure\Migrations
SET StartupProject=.\%ProjectName%.csproj

REM --- Commands ---

echo Adding Initial Migration...
dotnet ef migrations add InitialCreate --context %DbContextNamespace% --output-dir %OutputDir% --startup-project %StartupProject% --project %StartupProject%
if errorlevel 1 goto error

echo Updating Database...
dotnet ef database update --context %DbContextNamespace% --startup-project %StartupProject% --project %StartupProject%
if errorlevel 1 goto error

echo Done!

goto end

:error
echo An error occurred during the migration process. Please check the output above.

:end
pause