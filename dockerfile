FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 4444

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MystMovie.Test/MystMovie.Test.csproj", "MystMovie.Test/"]
RUN dotnet restore "MystMovie.Test/MystMovie.Test.csproj"
COPY . .
WORKDIR "/src/MystMovie.TorrentService"
RUN dotnet build "MystMovie.TorrentService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "MystMovie.TorrentService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MystMovie.TorrentService.dll"]