FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base

USER root

# curl is used by the compose healthcheck.
# /app/uploads is the shared staging folder for hotel images; it must be owned by the
# non-root app user, otherwise the (root-owned) volume mount makes it read-only for the API.
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/* \
    && mkdir -p /app/uploads \
    && chown -R $APP_UID:$APP_UID /app/uploads

USER $APP_UID

WORKDIR /app

EXPOSE 8080
EXPOSE 8081


FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

ARG BUILD_CONFIGURATION=Release

WORKDIR /src

# Restore only depends on the project files, so this layer stays cached until a csproj changes.
COPY ["src/Triply.Api/Triply.Api.csproj", "src/Triply.Api/"]
COPY ["src/Triply.Application/Triply.Application.csproj", "src/Triply.Application/"]
COPY ["src/Triply.Domain/Triply.Domain.csproj", "src/Triply.Domain/"]
COPY ["src/Triply.Infrastructure/Triply.Infrastructure.csproj", "src/Triply.Infrastructure/"]
COPY ["libraries/MessageQueue/MessageQueue.csproj", "libraries/MessageQueue/"]
COPY ["libraries/Logging/Logging.csproj", "libraries/Logging/"]

RUN dotnet restore "src/Triply.Api/Triply.Api.csproj"

# Copy only the projects the API needs, so edits in workers/ don't invalidate this image.
COPY ["src/Triply.Api/", "src/Triply.Api/"]
COPY ["src/Triply.Application/", "src/Triply.Application/"]
COPY ["src/Triply.Domain/", "src/Triply.Domain/"]
COPY ["src/Triply.Infrastructure/", "src/Triply.Infrastructure/"]
COPY ["libraries/MessageQueue/", "libraries/MessageQueue/"]
COPY ["libraries/Logging/", "libraries/Logging/"]

# publish already compiles, a separate "dotnet build" step would compile everything twice.
RUN dotnet publish "src/Triply.Api/Triply.Api.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false


FROM base AS final

WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Triply.Api.dll"]
