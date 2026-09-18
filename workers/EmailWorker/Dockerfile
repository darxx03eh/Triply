FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base

USER $APP_UID

WORKDIR /app


FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

ARG BUILD_CONFIGURATION=Release

WORKDIR /src

COPY ["workers/EmailWorker/EmailWorker.csproj", "workers/EmailWorker/"]
COPY ["libraries/MessageQueue/MessageQueue.csproj", "libraries/MessageQueue/"]

RUN dotnet restore "workers/EmailWorker/EmailWorker.csproj"

# Only the worker and the shared library: edits under src/ don't rebuild this image.
COPY ["workers/EmailWorker/", "workers/EmailWorker/"]
COPY ["libraries/MessageQueue/", "libraries/MessageQueue/"]

RUN dotnet publish "workers/EmailWorker/EmailWorker.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false


FROM base AS final

WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "EmailWorker.dll"]
