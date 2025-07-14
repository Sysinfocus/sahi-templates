@echo off
cd ProjectName.Data.Sqlite
dotnet ef migrations add %1 --startup-project ..\ProjectName.API
cd ..