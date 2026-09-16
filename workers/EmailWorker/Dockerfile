FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base

USER $APP_UID

WORKDIR /app


FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

ARG BUILD_CONFIGURATION=Release

WORKDIR /src

COPY ["workers/EmailWorker/EmailWorker.csproj", "workers/EmailWorker/"]
COPY ["libraries/MessageQueue/MessageQueue.csproj", "libraries/MessageQueue/"]

RUN dotnet restore "workers/EmailWorker/EmailWorker.csproj"

COPY . .

WORKDIR "/src/workers/EmailWorker"

RUN dotnet build "EmailWorker.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/build


FROM build AS publish

ARG BUILD_CONFIGURATION=Release

RUN dotnet publish "EmailWorker.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false


FROM base AS final

WORKDIR /app

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "EmailWorker.dll"]