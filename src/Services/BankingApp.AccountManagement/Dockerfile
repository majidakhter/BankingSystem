# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Services/BankingApp.AccountManagement/BankingApp.AccountManagement.csproj", "src/Services/BankingApp.AccountManagement/"]
COPY ["src/Core/BankingDDD.Application/Applications/Applications.csproj", "src/Core/BankingDDD.Application/Applications/"]
COPY ["src/Core/BankingDDD.Domain/Domains/Domains.csproj", "src/Core/BankingDDD.Domain/Domains/"]
COPY ["src/Core/BankingDDD.CommonLibrary/Common/Common.csproj", "src/Core/BankingDDD.CommonLibrary/Common/"]
COPY ["src/Core/BankingDDD.Infrastructure/Infrastructures/Infrastructures.csproj", "src/Core/BankingDDD.Infrastructure/Infrastructures/"]
RUN dotnet restore "./src/Services/BankingApp.AccountManagement/BankingApp.AccountManagement.csproj"
COPY . .
WORKDIR "/src/src/Services/BankingApp.AccountManagement"
RUN dotnet build "./BankingApp.AccountManagement.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BankingApp.AccountManagement.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BankingApp.AccountManagement.dll"]