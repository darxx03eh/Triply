FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base

USER $APP_UID

WORKDIR /app


FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

ARG BUILD_CONFIGURATION=Release

WORKDIR /src

COPY ["workers/ExpirePendingBookings/ExpirePendingBookings.csproj", "workers/ExpirePendingBookings/"]
COPY ["src/Triply.Application/Triply.Application.csproj", "src/Triply.Application/"]
COPY ["src/Triply.Domain/Triply.Domain.csproj", "src/Triply.Domain/"]
COPY ["src/Triply.Infrastructure/Triply.Infrastructure.csproj", "src/Triply.Infrastructure/"]
COPY ["libraries/Logging/Logging.csproj", "libraries/Logging/"]
COPY ["libraries/MessageQueue/MessageQueue.csproj", "libraries/MessageQueue/"]

RUN dotnet restore "workers/ExpirePendingBookings/ExpirePendingBookings.csproj"

COPY ["workers/ExpirePendingBookings/", "workers/ExpirePendingBookings/"]
COPY ["src/Triply.Application/", "src/Triply.Application/"]
COPY ["src/Triply.Domain/", "src/Triply.Domain/"]
COPY ["src/Triply.Infrastructure/", "src/Triply.Infrastructure/"]
COPY ["libraries/Logging/", "libraries/Logging/"]
COPY ["libraries/MessageQueue/", "libraries/MessageQueue/"]

RUN dotnet publish "workers/ExpirePendingBookings/ExpirePendingBookings.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false


FROM base AS final

WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "ExpirePendingBookings.dll"]