# См. статью по ссылке https://aka.ms/customizecontainer, чтобы узнать как настроить контейнер отладки и как Visual Studio использует этот Dockerfile для создания образов для ускорения отладки.

# Этот этап используется при запуске из VS в быстром режиме (по умолчанию для конфигурации отладки)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

RUN mkdir -p /app/wwwroot/storage && \
    chown -R 1000:1000 /app && \
    chown -R 1000:1000 /app/wwwroot && \
    chown -R 1000:1000 /app/wwwroot/storage && \
    chmod -R 755 /app/wwwroot/storage

# Этот этап используется для сборки проекта службы
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Debug
WORKDIR /src
COPY ["Directory.Packages.props", "."]
COPY ["Directory.Build.props", "."]
COPY ["src/Web/Web.csproj", "src/Web/"]
COPY ["src/Application/Application.csproj", "src/Application/"]
COPY ["src/Domain/Domain.csproj", "src/Domain/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "src/Infrastructure/"]
RUN dotnet restore "./src/Web/Web.csproj"
COPY . .
WORKDIR "/src/src/Web"
RUN dotnet build "./Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Этот этап используется для публикации проекта службы, который будет скопирован на последний этап
FROM build AS publish
ARG BUILD_CONFIGURATION=Debug
RUN dotnet publish "./Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false


# Этот этап используется в рабочей среде или при запуске из VS в обычном режиме (по умолчанию, когда конфигурация отладки не используется)
FROM base AS final
USER 1000
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Web.dll", "--urls", "http://0.0.0.0:8080"]