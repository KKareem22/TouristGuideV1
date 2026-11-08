@echo off
dotnet build TouristGuide.Application\TouristGuide.Application.csproj > build-app.txt 2>&1
dotnet build TouristGuide.Infrastructure\TouristGuide.Infrastructure.csproj > build-infra.txt 2>&1
dotnet build TouristGuide.API\TouristGuide.API.csproj > build-api.txt 2>&1
