# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["CommentsApp.API/CommentsApp.API.csproj", "CommentsApp.API/"]
COPY ["CommentsApp.Application/CommentsApp.Application.csproj", "CommentsApp.Application/"]
COPY ["CommentsApp.Domain/CommentsApp.Domain.csproj", "CommentsApp.Domain/"]
COPY ["CommentsApp.Infrastructure/CommentsApp.Infrastructure.csproj", "CommentsApp.Infrastructure/"]
RUN dotnet restore "./CommentsApp.API/CommentsApp.API.csproj"
COPY . .
WORKDIR "/src/CommentsApp.API"
RUN dotnet build "./CommentsApp.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CommentsApp.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

RUN mkdir -p /app/data /app/wwwroot/uploads && chown -R $APP_UID:$APP_UID /app/data /app/wwwroot

USER $APP_UID
ENTRYPOINT ["dotnet", "CommentsApp.API.dll"]