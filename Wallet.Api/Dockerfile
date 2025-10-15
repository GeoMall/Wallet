FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["Wallet.Api/Wallet.Api.csproj", "Wallet.Api/"]
COPY ["Wallet.Database/Wallet.Database.csproj", "Wallet.Database/"]
COPY ["Wallet.Models/Wallet.Models.csproj", "Wallet.Models/"]
COPY ["Wallet.Service/Wallet.Service.csproj", "Wallet.Service/"]
COPY ["Wallet.Client/Wallet.Client.csproj", "Wallet.Client/"]
COPY ["Wallet.Scheduler/Wallet.Scheduler.csproj", "Wallet.Scheduler/"]

RUN dotnet restore "Wallet.Api/Wallet.Api.csproj"

COPY . .
WORKDIR "/src/Wallet.Api"
RUN dotnet build "./Wallet.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Wallet.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Wallet.Api.dll"]
