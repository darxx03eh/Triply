FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base

USER root

RUN apt-get update \
    && apt-get install -y curl \
    && rm -rf /var/lib/apt/lists/*

USER $APP_UID

WORKDIR /app

EXPOSE 8080
EXPOSE 8081


FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

ARG BUILD_CONFIGURATION=Release

WORKDIR /src

COPY ["src/Triply.Api/Triply.Api.csproj", "src/Triply.Api/"]
COPY ["src/Triply.Application/Triply.Application.csproj", "src/Triply.Application/"]
COPY ["src/Triply.Domain/Triply.Domain.csproj", "src/Triply.Domain/"]
COPY ["src/Triply.Infrastructure/Triply.Infrastructure.csproj", "src/Triply.Infrastructure/"]
COPY ["libraries/MessageQueue/MessageQueue.csproj", "libraries/MessageQueue/"]

RUN dotnet restore "src/Triply.Api/Triply.Api.csproj"

COPY . .

WORKDIR "/src/src/Triply.Api"

RUN dotnet build "Triply.Api.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/build


FROM build AS publish

ARG BUILD_CONFIGURATION=Release

RUN dotnet publish "Triply.Api.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false


FROM base AS final

WORKDIR /app

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Triply.Api.dll"]