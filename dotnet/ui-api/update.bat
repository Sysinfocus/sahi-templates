@echo off
cd ProjectName.Data.Sqlite
dotnet ef database update --startup-project ..\ProjectName.API
cd ..