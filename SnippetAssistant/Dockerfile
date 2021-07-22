FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["SnippetAssistant/SnippetAssistant.csproj", "SnippetAssistant/"]
RUN dotnet restore "SnippetAssistant/SnippetAssistant.csproj"
COPY . .
WORKDIR "/src/SnippetAssistant"
RUN dotnet build "SnippetAssistant.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SnippetAssistant.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SnippetAssistant.dll"]
