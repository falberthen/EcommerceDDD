#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Crosscutting/EcommerceDDD.ApiGateway/EcommerceDDD.ApiGateway.csproj", "src/Crosscutting/EcommerceDDD.ApiGateway/"]
COPY ["src/Core/EcommerceDDD.Core.Infrastructure/EcommerceDDD.Core.Infrastructure.csproj", "src/Core/EcommerceDDD.Core.Infrastructure/"]
COPY ["src/Core/EcommerceDDD.Core/EcommerceDDD.Core.csproj", "src/Core/EcommerceDDD.Core/"]
RUN dotnet restore "./src/Crosscutting/EcommerceDDD.ApiGateway/EcommerceDDD.ApiGateway.csproj"
COPY . .
WORKDIR "/src/src/Crosscutting/EcommerceDDD.ApiGateway"
RUN dotnet build "EcommerceDDD.ApiGateway.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "EcommerceDDD.ApiGateway.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

# Create a non-root user and group
RUN addgroup --gid 1000 appgroup && adduser --uid 1000 --gid 1000 --home /app --disabled-password appuser

# Copy published output
COPY --from=publish /app/publish .

# Copy ocelot.json and set permissions
COPY src/Crosscutting/EcommerceDDD.ApiGateway/ocelot.json /app/ocelot.json
RUN chown appuser:appgroup /app/ocelot.json && chmod 644 /app/ocelot.json

# Switch to the non-root user
USER appuser

# Start the application
ENTRYPOINT ["dotnet", "EcommerceDDD.ApiGateway.dll"]
