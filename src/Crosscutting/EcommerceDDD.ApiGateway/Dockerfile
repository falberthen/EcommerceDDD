FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

USER root
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Crosscutting/EcommerceDDD.ApiGateway/EcommerceDDD.ApiGateway.csproj", "src/Crosscutting/EcommerceDDD.ApiGateway/"]
COPY ["src/Core/EcommerceDDD.Core.Infrastructure/EcommerceDDD.Core.Infrastructure.csproj", "src/Core/EcommerceDDD.Core.Infrastructure/"]
COPY ["src/Core/EcommerceDDD.Core/EcommerceDDD.Core.csproj", "src/Core/EcommerceDDD.Core/"]
RUN dotnet restore "./src/Crosscutting/EcommerceDDD.ApiGateway/EcommerceDDD.ApiGateway.csproj"
COPY . .
WORKDIR "/src/src/Crosscutting/EcommerceDDD.ApiGateway"
RUN dotnet build "./EcommerceDDD.ApiGateway.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./EcommerceDDD.ApiGateway.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

RUN mkdir -p /app/Ocelot
COPY src/Crosscutting/EcommerceDDD.ApiGateway/Ocelot/ocelot.json /app/Ocelot/ocelot.json

# 1654 is the 'app' User's UID default on.NET 8/10
# Diving full permissions (777) to this folder to ensure ocelot file merge
RUN chown -R 1654:1654 /app && \
    chmod -R 777 /app/Ocelot

# Mudamos para o usuário padrão da imagem
USER 1654

ENTRYPOINT ["dotnet", "EcommerceDDD.ApiGateway.dll"]