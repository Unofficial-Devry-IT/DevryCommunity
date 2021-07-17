FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["DevryBot/DevryBot.csproj", "DevryBot/"]
RUN dotnet restore "DevryBot/DevryBot.csproj"
COPY . .
WORKDIR "/src/DevryBot"
RUN dotnet build "DevryBot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DevryBot.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DevryBot.dll"]
