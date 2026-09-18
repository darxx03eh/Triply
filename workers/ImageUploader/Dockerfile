# aspnet (not runtime) because the worker references Triply.Infrastructure (Identity, JwtBearer).
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base

USER root

# Shared staging folder for hotel images, owned by the non-root app user (see src/Dockerfile).
RUN mkdir -p /app/uploads \
    && chown -R $APP_UID:$APP_UID /app/uploads

USER $APP_UID

WORKDIR /app


FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

ARG BUILD_CONFIGURATION=Release

WORKDIR /src

COPY ["workers/ImageUploader/ImageUploader.csproj", "workers/ImageUploader/"]
COPY ["src/Triply.Application/Triply.Application.csproj", "src/Triply.Application/"]
COPY ["src/Triply.Domain/Triply.Domain.csproj", "src/Triply.Domain/"]
COPY ["src/Triply.Infrastructure/Triply.Infrastructure.csproj", "src/Triply.Infrastructure/"]
COPY ["libraries/MessageQueue/MessageQueue.csproj", "libraries/MessageQueue/"]

RUN dotnet restore "workers/ImageUploader/ImageUploader.csproj"

COPY ["workers/ImageUploader/", "workers/ImageUploader/"]
COPY ["src/Triply.Application/", "src/Triply.Application/"]
COPY ["src/Triply.Domain/", "src/Triply.Domain/"]
COPY ["src/Triply.Infrastructure/", "src/Triply.Infrastructure/"]
COPY ["libraries/MessageQueue/", "libraries/MessageQueue/"]

RUN dotnet publish "workers/ImageUploader/ImageUploader.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false


FROM base AS final

WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "ImageUploader.dll"]
