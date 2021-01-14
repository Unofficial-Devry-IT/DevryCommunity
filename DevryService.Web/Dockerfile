#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["DevryService.Web/DevryService.Web.csproj", "DevryService.Web/"]
RUN dotnet restore "DevryService.Web/DevryService.Web.csproj"
COPY . .
WORKDIR "/src/DevryService.Web"
RUN dotnet build "DevryService.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DevryService.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DevryService.Web.dll"]