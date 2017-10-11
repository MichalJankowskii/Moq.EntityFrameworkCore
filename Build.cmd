@echo off

cd src
dotnet restore
dotnet build --configuration=Release
dotnet test ./Moq.EntityFrameworkCore.Tests/Moq.EntityFrameworkCore.Tests.csproj
dotnet test ./Moq.EntityFrameworkCore.Examples/Moq.EntityFrameworkCore.Examples.csproj
cd ..

if "%1"=="Publish" goto publish
goto end

:publish
powershell.exe -noprofile .build\publish-nuget-packages.ps1
goto end

:end