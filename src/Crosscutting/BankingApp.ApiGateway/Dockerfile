# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Switch to root user before installing curl
USER root 
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Crosscutting/BankingApp.ApiGateway/BankingApp.ApiGateway.csproj", "src/Crosscutting/BankingApp.ApiGateway/"]
COPY ["src/Core/BankingDDD.CommonLibrary/Common/Common.csproj", "src/Core/BankingDDD.CommonLibrary/Common/"]
RUN dotnet restore "./src/Crosscutting/BankingApp.ApiGateway/BankingApp.ApiGateway.csproj"
COPY . .
WORKDIR "/src/src/Crosscutting/BankingApp.ApiGateway"
RUN dotnet build "./BankingApp.ApiGateway.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BankingApp.ApiGateway.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app

# Create a non-root user and group
RUN addgroup --gid 1000 appgroup && adduser --uid 1000 --gid 1000 --home /app --disabled-password appuser

COPY --from=publish /app/publish .

# Copy ocelot.json and set permissions
COPY src/Crosscutting/BankingApp.ApiGateway/Ocelot/ocelot.json /app/Ocelot/ocelot.json
RUN chown appuser:appgroup /app/Ocelot/ocelot.json && chmod 644 /app/Ocelot/ocelot.json

# Switch to the non-root user
USER appuser

# Start the application

ENTRYPOINT ["dotnet", "BankingApp.ApiGateway.dll"]