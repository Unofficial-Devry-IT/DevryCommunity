FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

# the following folder must exist because it is part of the configuration
RUN mkdir -p /app/Data/Configs
RUN chmod --recursive 777 /app/Data
RUN chmod --recursive +x /app/Data

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["DevryBot/DevryBot.csproj", "DevryBot.csproj"]
RUN dotnet restore "DevryBot/DevryBot.csproj"
COPY . .

WORKDIR "/src/DevryBot"
RUN dotnet build "DevryBot/DevryBot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DevryBot/DevryBot.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DevryBot.dll"]
