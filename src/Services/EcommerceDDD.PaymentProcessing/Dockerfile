#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Services/EcommerceDDD.PaymentProcessing/EcommerceDDD.PaymentProcessing.csproj", "src/Services/EcommerceDDD.PaymentProcessing/"]
COPY ["src/Core/EcommerceDDD.Core.Infrastructure/EcommerceDDD.Core.Infrastructure.csproj", "src/Core/EcommerceDDD.Core.Infrastructure/"]
COPY ["src/Core/EcommerceDDD.Core/EcommerceDDD.Core.csproj", "src/Core/EcommerceDDD.Core/"]
RUN dotnet restore "./src/Services/EcommerceDDD.PaymentProcessing/./EcommerceDDD.PaymentProcessing.csproj"
COPY . .
WORKDIR "/src/src/Services/EcommerceDDD.PaymentProcessing"
RUN dotnet build "./EcommerceDDD.PaymentProcessing.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./EcommerceDDD.PaymentProcessing.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EcommerceDDD.PaymentProcessing.dll"]