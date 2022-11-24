FROM docker pull mcr.microsoft.com/dotnet/aspnet:6.0 AS 'base'
WORKDIR /app

# the following folder must exist because it is part of the configuration
RUN mkdir -p /app/Data/Configs
RUN chmod --recursive 777 /app/Data
RUN chmod --recursive +x /app/Data

# Python Snippet Scanner Requires the following items to be installed
RUN apt-get update
RUN apt-get install -y python3.11
RUN apt install -y python3-pip
RUN pip3 install prospector[with_everything]

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
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
