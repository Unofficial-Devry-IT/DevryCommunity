#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This image shall be used to "run" the application
FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

# This is used to compile the application
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["./DevryService.csproj", "Web/"]

# Add the DSharpPlus nuget package source
RUN dotnet nuget add source https://nuget.emzi0767.com/api/v3/index.json --name DSharpPlus

# Restore all nuget packages for this project
RUN dotnet restore "./DevryService.csproj"
COPY . .
WORKDIR "/src/DevryService"

# Build the application in "Release" configuration
RUN dotnet build "DevryService.csproj" -c Release -o /app/build

# Publish the application to our /app directory
FROM build AS publish
RUN dotnet publish "Web.csproj" -c Release -o /app/publish

# Update permissions
RUN chmod -R 777 /app/publish

# Final part of image that gets deployed to dockerhub
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DevryService.dll"]