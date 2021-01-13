#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["DevryService/DevryService.csproj", "DevryService/"]
RUN dotnet restore "DevryService/DevryService.csproj"
COPY . .
WORKDIR "/src/DevryService"
RUN dotnet build "DevryService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DevryService.csproj" -c Release -o /app/publish

COPY ["DevryService/DevryCommunity.db", "/app/publish"]
ADD DevryService/Snippets /app/publish

RUN chmod -R 777 /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DevryService.dll"]
